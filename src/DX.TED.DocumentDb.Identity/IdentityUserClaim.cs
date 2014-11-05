using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.TED.DocumentDb.Identity
{
    public class IdentityUserClaim<TKey>
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }

        public virtual TKey UserId { get; set; }

    }
}
