using Quartz;
using Quartz.Impl;
using SAPAutomationJob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RobotSAPAutomationWindowsServiceHost
{
    public partial class RobotService : ServiceBase
    {
        #region Declarations

        private StdSchedulerFactory _SchedulerFactory;
        private IScheduler _Scheduler;

        #endregion Declarations

        public RobotService()
        {
            InitializeComponent();
            _SchedulerFactory = new StdSchedulerFactory();
            _Scheduler = _SchedulerFactory.GetScheduler();
            var job = JobBuilder.Create<SAPAutomationJob.SAPAutomationJob>().WithIdentity("name", "group").Build();
            var cronSchedule = ConfigurationManager.AppSettings["WakeUpTimeCron"];
            var trigger = TriggerBuilder.Create().WithIdentity("Crontrigger")
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule)).Build();
            _Scheduler.ScheduleJob(job, trigger);

            _Scheduler.Start();
        }

        protected override void OnStart(string[] args)
        {
            if (_Scheduler == null) return;
           // _Scheduler.Start();
        }

        protected override void OnStop()
        {
            if (_Scheduler == null) return;
            _Scheduler.Shutdown();
        }
    }
}
