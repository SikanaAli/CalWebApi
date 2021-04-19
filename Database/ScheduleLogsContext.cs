using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CalWebApi.Database
{
    public class ScheduleLogsContext : DbContext
    {
        public DbSet<ScheduleLogsModel> ExecutionLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=AppDb.db3");
            
    }
}
