///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// ParameterSource.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using NSFW.PlxSensors;

namespace NateW.Ssm
{
    public class PlxParameter : Parameter
    {
        private PlxSensorId sensorId;

        public PlxSensorId SensorId
        {
            get
            {
                return this.sensorId;
            }
        }

        public PlxParameter(
            ParameterSource source,
            PlxSensorId sensorId, 
            string id, 
            string name, 
            ReadOnlyCollection<Conversion> conversions)
            : base(
            source,
            id,
            name, 
            conversions,
            null)
        {
            this.sensorId = sensorId;
        }
    }

    [CLSCompliant(true)]
    public class PlxParameterSource : ParameterSource
    {
        private PlxParameterSource() : base ("PLX")
        {
            this.Initialize();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static PlxParameterSource GetInstance()
        {
            return new PlxParameterSource();
        }

        private void Initialize()
        {
            List<Conversion> conversions = new List<Conversion>();
            conversions.Add(Conversion.GetInstance("Lambda", "(x / 3.75 + 68) / 100", "0.00"));
            conversions.Add(Conversion.GetInstance("Gasoline AFR", "(x / 2.55 + 100) / 10", "0.00"));

            Parameter parameter = new PlxParameter(
                this,
                new PlxSensorId(PlxSensorType.WidebandAfr, 0), 
                "PlxMfdWB1", 
                "PLX Wideband O2", 
                conversions.AsReadOnly());
            
            this.AddParameter(parameter);
            conversions.Clear();

            conversions = new List<Conversion>();
            conversions.Add(Conversion.GetInstance("C", "x", "0.00"));
            conversions.Add(Conversion.GetInstance("F", "x / .555 + 32", "0.00"));

            parameter = new PlxParameter(
                this,
                new PlxSensorId(PlxSensorType.ExhaustGasTemperature, 0),
                "PlxMfdEGT1", 
                "PLX Exhaust Gas Temperature", 
                conversions.AsReadOnly());

            this.AddParameter(parameter);
            conversions.Clear();
        }
    }
}