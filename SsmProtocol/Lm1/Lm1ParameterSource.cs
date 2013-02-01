///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// Lm1ParameterSource.cs
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
/*using NSFW.Lm1Sensors;

namespace NateW.Ssm
{
    public class Lm1Parameter : Parameter
    {
        private Lm1SensorId sensorId;

        public Lm1SensorId SensorId
        {
            get
            {
                return this.sensorId;
            }
        }

        public Lm1Parameter(
            ParameterSource source,
            Lm1SensorId sensorId, 
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
    public class Lm1ParameterSource : ParameterSource
    {
        private Lm1ParameterSource() : base ("Lm1")
        {
            this.Initialize();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static Lm1ParameterSource GetInstance()
        {
            return new Lm1ParameterSource();
        }

        private void Initialize()
        {
            List<Conversion> conversions = new List<Conversion>();
            conversions.Add(Conversion.GetInstance("Lambda", "(x / 3.75 + 68) / 100", "0.00"));
            conversions.Add(Conversion.GetInstance("Gasoline AFR", "(x / 2.55 + 100) / 10", "0.00"));

            Parameter parameter = new Lm1Parameter(
                this,
                new Lm1SensorId(Lm1SensorType.WidebandAfr, 0), 
                "Lm1MfdWB1", 
                "Lm1 Wideband O2", 
                conversions.AsReadOnly());
            
            this.AddParameter(parameter);
            conversions.Clear();

            conversions = new List<Conversion>();
            conversions.Add(Conversion.GetInstance("C", "x", "0.00"));
            conversions.Add(Conversion.GetInstance("F", "x / .555 + 32", "0.00"));

            parameter = new Lm1Parameter(
                this,
                new Lm1SensorId(Lm1SensorType.ExhaustGasTemperature, 0),
                "Lm1MfdEGT1", 
                "Lm1 Exhaust Gas Temperature", 
                conversions.AsReadOnly());

            this.AddParameter(parameter);
            conversions.Clear();
        }
    }
}*/