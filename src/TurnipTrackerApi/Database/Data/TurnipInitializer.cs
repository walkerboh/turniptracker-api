using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TurnipTallyApi.Database.Entities;

namespace TurnipTallyApi.Database.Data
{
    public static class TurnipInitializer
    {
        public static void Initialize(TurnipContext context, IWebHostEnvironment env)
        {
            context.Database.EnsureCreated();

            if(!context.Timezones.Any())
            {
                var tzs = JsonConvert.DeserializeObject<IEnumerable<Timezone>>(File.ReadAllText(Path.Combine(env.ContentRootPath, "database", "data",
                    "timezones.json")));

                context.Timezones.AddRange(tzs);
                context.SaveChanges();
            }
        }
    }
}