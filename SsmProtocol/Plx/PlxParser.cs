///////////////////////////////////////////////////////////////////////////////
// Contributed by NSFW from the RomRaider forum.  See this thread for details:
// http://www.romraider.com/forum/topic4299.html
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using NateW.Ssm;

namespace NSFW.PlxSensors
{
    /// <summary>
    /// Types of sensors supported by this PLX protocol.
    /// </summary>
    /// <remarks>
    /// Based on the table in PLXApp018.pdf.
    /// </remarks>
    public enum PlxSensorType
    {
        [DisplayName("Wideband AFR")]
        [ParameterId("PlxWBO2")]
        WidebandAfr = 0,

        [DisplayName("Exhaust gas temperature")]
        [ParameterId("PlxEGT")]
        ExhaustGasTemperature = 1,

        FluidTemperature = 2,
        Vacuum = 3,
        Boost = 4,
        AirIntakeTemperature = 5,
        Rpm = 6,
        VehicleSpeed = 7,
        ThrottlePosition = 8,
        EngineLoad = 9,
        FluidPressure = 10,
        Timing = 11,
        ManifoldAbsolutePressure = 12,
        MassAirFlow = 13,
        ShortTermFuelTrim = 14,
        LongTermFuelTrim = 15,
        NarrowbandAfr = 16,
        FuelLevel = 17,
        Voltage = 18,
        Knock = 19,
        DutyCycle = 20,

        /// <summary>
        /// Unknown device.
        /// </summary>
        /// <remarks>
        /// This is not described in the protocol spec, but was observed when
        /// testing with only a SM-AFR and DM-5.
        /// </remarks>
        Unknown = 4032,
    }

    /// <summary>
    /// Units for conversion of raw values from PLX sensors.
    /// </summary>
    /// <remarks>
    /// Based on the conversion code in PLXApp018.pdf.
    /// This is not a complete list, feel free to add more.
    /// </remarks>
    public enum PlxSensorUnits
    {
        Raw = -1,

        [DisplayNameAttribute("Lambda")]
        WidebandAfrLambda = 0,

        [DisplayNameAttribute("Gasoline AFR")]
        WidebandAfrGasoline147 = 1,

        WidebandAfrDiesel146 = 2,
        WidebandAfrMethanol64 = 3,
        WidebandAfrEthanol90 = 4,
        WidebandAfrLpg155 = 5,
        WidebandAfrCng172 = 6,

        [DisplayNameAttribute("C")]
        ExhaustGasTemperatureCelsius = 0,
        [DisplayNameAttribute("F")]
        ExhaustGasTemperatureFahrenheit = 1,

        AirIntakeTemperatureCelsius = 0,
        AirIntakeTemperatureFahrenheit = 1,
        Knock = 0,
    }

    /// <summary>
    /// Identifies a PLX Devices sensor type and instance.
    /// </summary>
    public struct PlxSensorId
    {
        public readonly PlxSensorType Sensor;
        public readonly int Instance;
        public PlxSensorId(PlxSensorType sensor, int instance)
        {
            Sensor = sensor;
            Instance = instance;
        }

        /// <summary>
        /// FxCop recommended doing this...
        /// </summary>
        public override bool Equals(object obj)
        {            
            if (obj is PlxSensorId)
            {
                PlxSensorId that = (PlxSensorId) obj;
                if ((this.Sensor == that.Sensor) &&
                    (this.Instance == that.Instance))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// FxCop recommended...
        /// </summary>
        public override int GetHashCode()
        {
            return (this.Instance << 16) | (int)this.Sensor;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
        public static bool operator== (PlxSensorId a, PlxSensorId b)
        {
            return a.Equals(b);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
        public static bool operator!= (PlxSensorId a, PlxSensorId b)
        {
            return !a.Equals(b);
        }
    }

    /// <summary>
    /// Parses data from PLX Devices serial streams.
    /// </summary>
    /// <remarks>
    /// Based on the protocol description in PLXApp018.pdf.
    /// </remarks>
    class PlxParser
    {
        /// <summary>
        /// Current state of the parser.
        /// </summary>
        private enum ParserState
        {
            ExpectingStart,
            ExpectingFirstHalfOfSensorType,
            ExpectingSecondHalfOfSensorType,
            ExpectingInstance,
            ExpectingFirstHalfOfValue,
            ExpectingSecondHalfOfValue,
        }

        /// <summary>
        /// Dictionary of the most recent results received so far.
        /// </summary>
        private Dictionary<PlxSensorId, int> results;

        /// <summary>
        /// Storage for incomplete data in transmission.
        /// </summary>
        private int partialValue;

        /// <summary>
        /// Current parser state.
        /// </summary>
        private ParserState state = ParserState.ExpectingStart;

        /// <summary>
        /// Current sensor type (not always valid, depending on parser state).
        /// </summary>
        private PlxSensorType sensorType;

        /// <summary>
        /// Current sensor instance (not always valid, depending on parser state).
        /// </summary>
        private int instance;

        /// <summary>
        /// Used to synchronize access to the dictionary.
        /// </summary>
        private object dictionaryLock = new object();

        /// <summary>
        /// Returns the set of sensors that have reported values so far.
        /// </summary>
        public IEnumerable<PlxSensorId> Sensors
        {
            get
            {
                return this.results.Keys;
            }
        }

        /// <summary>
        /// Creates a parser.
        /// </summary>
        public PlxParser()
        {
            this.results = new Dictionary<PlxSensorId, int>();
        }

        /// <summary>
        /// Call this for each byte received from the PLX chain.
        /// </summary>
        /// <param name="b">Received byte.</param>
        /// <returns>If a complete packet was received, this will return the 
        /// PlxSensorId of the sensor whose data has been updated.  Call the
        /// GetValue() method to retrieve the value.</returns>
        public PlxSensorId? PushByte(byte b)
        {
            if (b == 0x80)
            {
                this.state = ParserState.ExpectingFirstHalfOfSensorType;
                return null;
            }

            if (b == 0x40)
            {
                this.state = ParserState.ExpectingStart;
                return null;
            }

            switch (this.state)
            {
                case ParserState.ExpectingFirstHalfOfSensorType:
                    this.partialValue = b;
                    this.state = ParserState.ExpectingSecondHalfOfSensorType;
                    break;

                case ParserState.ExpectingSecondHalfOfSensorType:
                    this.sensorType = (PlxSensorType)((this.partialValue << 6) | b);
                    this.state = ParserState.ExpectingInstance;
                    break;

                case ParserState.ExpectingInstance:
                    this.instance = b;
                    this.state = ParserState.ExpectingFirstHalfOfValue;
                    break;

                case ParserState.ExpectingFirstHalfOfValue:
                    this.partialValue = b;
                    this.state = ParserState.ExpectingSecondHalfOfValue;
                    break;

                case ParserState.ExpectingSecondHalfOfValue:
                    int rawValue = (this.partialValue << 6) | b;
                    //double convertedValue = PlxParser.ConvertValue(this.sensorType, i);
                    PlxSensorId id = new PlxSensorId(this.sensorType, this.instance);
                    lock (this.dictionaryLock)
                    {
                        this.results[id] = rawValue;
                    }
                    this.state = ParserState.ExpectingFirstHalfOfSensorType;
                    return id;
            }
            return null;
        }

        /// <summary>
        /// Use this to retrieve a sensor's value in the units of your choice.
        /// </summary>
        /// <param name="sensorId">PlxSensorId returned from PushByte()</param>
        /// <param name="units">Desired units.</param>
        /// <returns>The sensor's value in the desired units.</returns>
        public double GetValue(PlxSensorId sensorId, PlxSensorUnits units)
        {
            int rawValue;
            lock (this.dictionaryLock)
            {
                if (!this.results.ContainsKey(sensorId))
                {
                    return 0;
                }
                rawValue = this.results[sensorId];
            }

            double result;
            if (units == PlxSensorUnits.Raw)
            {
                result = rawValue;
            }
            else
            {
                result = PlxParser.ConvertValue(sensorId.Sensor, units, rawValue);
            }

            return result;
        }

        /// <summary>
        /// Convert the given raw value to the desired units, according to sensor type.
        /// </summary>
        /// <remarks>
        /// This code was copy/pasted from PLXApp018.PDF, with very minor changes (the 
        /// original used integers for sensor type and units).
        /// </remarks>
        /// <param name="sensorType">Type of sensor.</param>
        /// <param name="units">Desired units.</param>
        /// <param name="raw">Raw value from the sensor.</param>
        /// <returns>Value converted to the desired units.</returns>
        private static double ConvertValue(PlxSensorType sensorType, PlxSensorUnits units, int raw)
        {
            double data = 0;
            if (sensorType == PlxSensorType.WidebandAfr)
            {
                if (units == PlxSensorUnits.WidebandAfrLambda)
                    data = (raw / 3.75 + 68) / 100;
                else if (units == PlxSensorUnits.WidebandAfrGasoline147)
                    data = (raw / 2.55 + 100) / 10;
                else if (units == PlxSensorUnits.WidebandAfrDiesel146)
                    data = (raw / 2.58 + 100) / 10;
                else if (units == PlxSensorUnits.WidebandAfrMethanol64)
                    data = (raw / 5.856 + 43.5) / 10;
                else if (units == PlxSensorUnits.WidebandAfrEthanol90)
                    data = (raw / 4.167 + 61.7) / 10;
                else if (units == PlxSensorUnits.WidebandAfrLpg155)
                    data = (raw / 2.417 + 105.6) / 10;
                else if (units == PlxSensorUnits.WidebandAfrCng172)
                    data = (raw / 2.18 + 117) / 10;
            }
            else if (sensorType == PlxSensorType.ExhaustGasTemperature)
            {
                if (units == PlxSensorUnits.ExhaustGasTemperatureCelsius)
                    data = raw;
                else if (units == PlxSensorUnits.ExhaustGasTemperatureFahrenheit)
                    data = (raw / .555 + 32);
            }


/*                      Call me lazy, but I'm not going to polish code for sensors that I don't own.
             
                        else if (sensorType == PlxSensorType.FluidTemperature)
                        {
                            if (units == 0) //Degrees Celsius Water
                                data = raw;
                            else if (units == 1) //Degrees Fahrenheit Water
                                data = (raw / .555 + 32);
                            else if (units == 2) //Degrees Celsius Oil
                                data = raw;
                            else if (units == 3) //Degrees Fahrenheit Oil
                                data = (raw / .555 + 32);
                        }
                        else if (sensorType == PlxSensorType.Vacuum) //Vac
                        {
                            if (units == 0) //in/Hg (inch Mercury)
                                data = -(raw / 11.39 - 29.93);
                            else if (units == 1) //mm/Hg (millimeters Mercury)
                                data = -(raw * 2.23 + 760.4);
                        }
                        else if (sensorType == PlxSensorType.Boost) //Boost
                        {
                            if (units == 0) //0-30 PSI
                                data = raw / 22.73;
                            else if (units == 1) //0-2 kg/cm^2
                                data = raw / 329.47;
                            else if (units == 2) //0-15 PSI
                                data = raw / 22.73;
                            else if (units == 3) //0-1 kg/cm^2
                                data = raw / 329.47;
                            else if (units == 4) //0-60 PSI
                                data = raw / 22.73;
                            else if (units == 5) //0-4 kg/cm^2
                                data = raw / 329.47;
                        }
                        else if (sensorType == PlxSensorType.AirIntakeTemperature) //AIT
                        {
                            if (units == 0) //Celsius
                                data = raw;
                            else if (units == 1) //Fahrenheit
                                data = (raw / .555 + 32);
                        }
                        else if (sensorType == PlxSensorType.Rpm) //RPM
                        {
                            data = raw * 19.55; //RPM
                        }
                        else if (sensorType == PlxSensorType.VehicleSpeed) //Speed
                        {
                            if (units == 0) //MPH
                                data = raw / 6.39;
                            else if (units == 1) //KMH
                                data = raw / 3.97;
                        }
                        else if (sensorType == PlxSensorType.ThrottlePosition) //TPS
                        {
                            data = raw; //Throttle Position %
                        }
                        else if (sensorType == PlxSensorType.EngineLoad) //Engine Load
                        {
                            data = raw; //Engine Load %
                        }
                        else if (sensorType == PlxSensorType.FluidPressure) //Fluid Pressure
                        {
                            if (units == 0) //PSI Fuel
                                data = raw / 5.115;
                            else if (units == 1) //kg/cm^2 Fuel
                                data = raw / 72.73;
                            else if (units == 2) //Bar Fuel
                                data = raw / 74.22;
                            else if (units == 3) //PSI Oil
                                data = raw / 5.115;
                            else if (units == 4) //kg/cm^2 Oil
                                data = raw / 72.73;
                            else if (units == 5) //Bar Oil
                                data = raw / 74.22;
                        }
                        else if (sensorType == PlxSensorType.Timing) //Engine timing
                        {
                            data = raw - 64; //Degree Timing
                        }
                        else if (sensorType == PlxSensorType.ManifoldAbsolutePressure) //MAP
                        {
                            if (units == 0) //kPa
                                data = raw;
                            else if (units == 1) //inHg
                                data = raw / 3.386;
                        }
                        else if (sensorType == PlxSensorType.MassAirFlow) //MAF
                        {
                            if (units == 0) //g/s (grams per second)
                                data = raw;
                            else if (units == 1) //lb/min (pounds per minute)
                                data = raw / 7.54;
                        }
                        else if (sensorType == PlxSensorType.ShortTermFuelTrim) //Short term fuel trim
                        {
                            data = raw - 100; //Fuel trim %
                        }
                        else if (sensorType == PlxSensorType.LongTermFuelTrim) //Long term fuel trim
                        {
                            data = raw - 100; //Fuel trim %
                        }
                        else if (sensorType == PlxSensorType.NarrowbandAfr) //Narrowband O2 sensor
                        {
                            if (units == 0) //Percent
                                data = raw;
                            else if (units == 1) //Volts
                                data = raw / 78.43;
                        }
                        else if (sensorType == PlxSensorType.FuelLevel) //Fuel level
                        {
                            data = raw; //Fuel Level %
                        }
                        else if (sensorType == PlxSensorType.Voltage) //Volts
                        {
                            data = raw / 51.15; //Volt Meter Volts
                        }
                        else if (sensorType == PlxSensorType.Knock) //Knock
                        {
                            data = raw / 204.6; //Knock volts 0-5
                        }
                        else if (sensorType == PlxSensorType.DutyCycle) //Duty cycle
                        {
                            if (units == 0) //Positive Duty
                                data = raw / 10.23;
                            else if (units == 1) //Negative Duty
                                data = 100 - (raw / 10.23);
                        }*/
            return data;
        }
    }
}
