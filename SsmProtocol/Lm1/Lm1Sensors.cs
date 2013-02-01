using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.IO.Ports;
//using NSFW.Lm1Sensors;
/*
namespace NateW.Ssm
{
    public class Lm1SensorEventArgs : EventArgs
    {
        public Lm1SensorId SensorId { get; private set; }

        public Lm1SensorEventArgs(Lm1SensorId sensorId)
        {
            this.SensorId = sensorId;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
    public class Lm1Sensors : IDisposable
    {
        private string portName;
        private Lm1Parser parser;
        private SuspendResumePort manager;
        private byte[] buffer;

        public event EventHandler<Lm1SensorEventArgs> ValueReceived;

        private Lm1Sensors(string portName)
        {
            this.parser = new Lm1Parser();
            this.portName = portName;
            this.buffer = new byte[1000];
            this.manager = new SuspendResumePort(
                this.StreamFactory,
                this.BeginRead);
            this.manager.Open();
        }

        ~Lm1Sensors()
        {
            this.Dispose(false);
        }

        public static Lm1Sensors GetInstance(string port)
        {
            Lm1Sensors sensors = new Lm1Sensors(port);
            return sensors;
        }

        public void Close()
        {
            if (this.manager != null)
            {
                this.manager.Close();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public double GetValue(Lm1SensorId id, Lm1SensorUnits units)
        {
            return this.parser.GetValue(id, units);
        }

        private SerialPort StreamFactory()
        {
            Trace.WriteLine("Lm1Sensors.StreamFactory invoked.");

            SerialPort port = new SerialPort(this.portName, 19200, Parity.None, 8, StopBits.One);
            port.ReadTimeout = 50;
            port.Open();
            return port;
        }

        private void BeginRead(Stream stream)
        {
            stream.BeginRead(this.buffer, 0, this.buffer.Length, this.ReadCompleted, stream);
        }

        private void ReadCompleted(IAsyncResult result)
        {
            int bytesRead = 0;
            Stream stream = (Stream) result.AsyncState;
            try
            {
                if (stream != null)
                {
                    bytesRead = stream.EndRead(result);
                }
                else
                {
                    Trace.WriteLine("??? Lm1Sensors.ReadCompleted: result.AsyncState (the Stream) is null.");
                }
            }
            catch (IOException ex)
            {
                Trace.WriteLine("Lm1Sensors.ReadCompleted: " + ex.ToString());
                Trace.WriteLine("Lm1Sensors.ReadCompleted requesting restart.");
                this.manager.Restart();
                return;
            }
                        
            for (int i = 0; i < bytesRead; i++)
            {
                Lm1SensorId? sensorId = this.parser.PushByte(this.buffer[i]);
                if ((sensorId.HasValue) && (this.ValueReceived != null))
                {
                    this.ValueReceived(this, new Lm1SensorEventArgs(sensorId.Value));
                }
            }

            this.manager.StartOperation();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
                if (this.manager != null)
                {
                    this.manager.Dispose();
                }
            }
        }
    }
}
*/