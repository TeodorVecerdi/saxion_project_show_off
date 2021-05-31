using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;

public class AWSScoreboardTest : MonoBehaviour {
    public int QueryCount = 4;

    public int AddScore;
    public string AddName;

    [Button("Query"),]
    private async Task QueryAWS() {
        using var httpClient = new HttpClient();
        var uri = new Uri($"https://edgjs1jvk1.execute-api.eu-central-1.amazonaws.com/default/query-scoreboard?count={QueryCount}");
        var response = await httpClient.GetAsync(uri);
        
        Debug.Log($"[{response.StatusCode}] {response.ReasonPhrase}");
        var json = await response.Content.ReadAsStringAsync();
        var objs = JsonConvert.DeserializeObject<ResponseData[]>(json);
        foreach (var responseData in objs) {
            Debug.Log(responseData);
        }
    }

    [Button("Insert")]
    private async Task AddAWS() {
        var guid = Guid.NewGuid().ToString();
        using var httpClient = new HttpClient();
        var uri = new Uri("https://edgjs1jvk1.execute-api.eu-central-1.amazonaws.com/default/add-scoreboard");
        var response = await httpClient.PutAsync(uri, new StringContent($"{{\"score\": \"{AddScore}\", \"name\": \"{AddName}\", \"guid\": \"{guid}\"}}", Encoding.UTF8));
        
        Debug.Log($"[{response.StatusCode}] {response.ReasonPhrase}");
        Debug.Log(await response.Content.ReadAsStringAsync());
    }

    internal class ResponseData {
        public string Guid { get; set; }
        public string Score { get; set; }
        public string PlayerName { get; set; }

        public ResponseData(string guid, string score, string playerName) {
            Guid = guid;
            Score = score;
            PlayerName = playerName;
        }

        public override string ToString() {
            return $"{Guid} ; {PlayerName} ; {Score}";
        }
    }
}