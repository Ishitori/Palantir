namespace Ix.Palantir.Scheduler.Runner
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading;
    using System.Xml;
    using Ix.Palantir.Logging;

    public class SchedulerServer : IDisposable
    {
        private bool disposed;

        public SchedulerServer()
        {
            this.mMutex = new Mutex();
            this.mKeepRunning = true;
            this.mAssemblyPaths = new ArrayList();
            this.mSchedulers = new ArrayList();
        }

        public void AddAssemblyPath(string assemblyPath)
        {
            this.mAssemblyPaths.Add(assemblyPath);
            log.Debug("Adding: " + assemblyPath + " to Assembly Paths");
        }
        public void AddScheduler(Scheduler scheduler)
        {
            if (this.mSchedulers == null)
            {
                this.mSchedulers = new ArrayList();
            }

            this.mSchedulers.Add(scheduler);
        }

        private void DoWork()
        {
            this.KeepRunning = true;
            log.DebugFormat("Scheduler server is running. Trying to run {0} scheduler instance(s)...", this.mSchedulers.Count);
            for (int num1 = 0; num1 < this.mSchedulers.Count; num1++)
            {
                ((Scheduler)this.mSchedulers[num1]).Start();
            }

            log.Debug("All scheduler instances has been run");
            while (this.KeepRunning)
            {
                Thread.Sleep(sSleepTime);
            }

            for (int num2 = 0; num2 < this.mSchedulers.Count; num2++)
            {
                ((Scheduler)this.mSchedulers[num2]).Stop();
            }
        }

        public void LoadFromXml(XmlNode node)
        {
            this.mSchedulers.Clear();
            XmlNode node1 = node["Schedulers"];
            XmlAttribute attribute1 = node1.Attributes["eventLogging"];
            if (attribute1 != null)
            {
                string text1 = attribute1.Value;
                string trueString = "true";
                trueString.Equals(text1);
            }

            XmlNodeList list1 = node1.SelectNodes("./AssemblyPaths");
            log.Debug("Current AppDomain path is: " + AppDomain.CurrentDomain.BaseDirectory);
            for (int num1 = 0; num1 < list1.Count; num1++)
            {
                XmlNode node2 = list1[num1];
                string text2 = node2.InnerText;
                this.AddAssemblyPath(text2);
                log.Debug("Adding: " + text2 + " to Assembly Paths");
            }

            XmlNodeList list2 = node1.SelectNodes("./Scheduler");
            for (int num2 = 0; num2 < list2.Count; num2++)
            {
                Scheduler scheduler1 = Scheduler.MakeFromXML(this.GetAssemblyPaths(), list2[num2]);
                this.mSchedulers.Add(scheduler1);
            }
        }
        public void LoadFromXmlFile(string filename)
        {
            Stream stream1 = File.OpenRead(filename);
            XmlDocument document1 = new XmlDocument();
            document1.Load(stream1);
            this.LoadFromXml(document1);
        }

        public void Run()
        {
            ThreadStart start1 = new ThreadStart(this.DoWork);
            new Thread(start1).Start();
        }
        public void Stop()
        {
            this.KeepRunning = false;
        }

        public string[] GetAssemblyPaths()
        {
            return (string[])this.mAssemblyPaths.ToArray(typeof(string));
        }

        private bool KeepRunning
        {
            get
            {
                this.mMutex.WaitOne();
                bool flag1 = this.mKeepRunning;
                this.mMutex.ReleaseMutex();
                return flag1;
            }

            set
            {
                this.mMutex.WaitOne();
                this.mKeepRunning = value;
                this.mMutex.ReleaseMutex();
            }
        }

        private static readonly ILog log = LogManager.GetLogger("Scheduler");
        private ArrayList mAssemblyPaths;
        private bool mKeepRunning;
        private Mutex mMutex;
        private ArrayList mSchedulers;
        private static int sSleepTime = 0x7d0;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mMutex", Justification = "Reviewed. Suppression is OK here.")]
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.mMutex != null)
                    {
                        this.mMutex.Dispose();
                    }
                }

                this.disposed = true;
            }
        }
    }
}