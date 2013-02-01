using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.IO.Ports;
using NSFW.PlxSensors;

namespace NateW.Ssm
{
    public class PlxSensorEventArgs : EventArgs
    {
        public PlxSensorId SensorId { get; private set; }

        public PlxSensorEventArgs(PlxSensorId sensorId)
        {
            this.SensorId = sensorId;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
    public class PlxSensors : IDisposable
    {
        private string portName;
        private PlxParser parser;
        private SuspendResumePort manager;
        private byte[] buffer;

        public event EventHandler<PlxSensorEventArgs> ValueReceived;

        private PlxSensors(string portName)
        {
            this.parser = new PlxParser();
            this.portName = portName;
            this.buffer = new byte[1000];
            this.manager = new SuspendResumePort(
                this.StreamFactory,
                this.BeginRead);
            this.manager.Open();
        }

        ~PlxSensors()
        {
            this.Dispose(false);
        }

        public static PlxSensors GetInstance(string port)
        {
            PlxSensors sensors = new PlxSensors(port);
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

        public double GetValue(PlxSensorId id, PlxSensorUnits units)
        {
            return this.parser.GetValue(id, units);
        }

        private SerialPort StreamFactory()
        {
            Trace.WriteLine("PlxSensors.StreamFactory invoked.");

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
                    Trace.WriteLine("??? PlxSensors.ReadCompleted: result.AsyncState (the Stream) is null.");
                }
            }
            catch (IOException ex)
            {
                Trace.WriteLine("PlxSensors.ReadCompleted: " + ex.ToString());
                Trace.WriteLine("PlxSensors.ReadCompleted requesting restart.");
                this.manager.Restart();
                return;
            }
                        
            for (int i = 0; i < bytesRead; i++)
            {
                PlxSensorId? sensorId = this.parser.PushByte(this.buffer[i]);
                if ((sensorId.HasValue) && (this.ValueReceived != null))
                {
                    this.ValueReceived(this, new PlxSensorEventArgs(sensorId.Value));
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
