using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questao2
{
    internal class PartidaServices
    {
        private string url = "https://jsonmock.hackerrank.com/api/football_matches";
        public async Task<PartidaResponse> Consulta(int year, string team1, string team2, int page=1)
        {
            var filtro = string.Empty;
            if (!string.IsNullOrEmpty(team1))
                filtro = string.Format("?year={0}&team1={1}&page={2}", year, team1, page);

            if (!string.IsNullOrEmpty(team2))
                filtro = string.Format("?year={0}&team2={1}&page={2}", year, team2, page);

            HttpClient client = new HttpClient();

            var uri = string.Format("{0}{1}", url, filtro);

            var response = client.GetAsync(uri).Result;

            if (response.IsSuccessStatusCode)
            {
                var results = response.Content.ReadAsStringAsync().Result;

                var list = JsonConvert.DeserializeObject<PartidaResponse>(results);

                return list;

            }
            return new PartidaResponse();

        }

    }
}
