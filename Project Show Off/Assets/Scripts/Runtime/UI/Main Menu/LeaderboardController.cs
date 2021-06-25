using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Runtime {
    public class LeaderboardController : MonoBehaviour {
        [SerializeField] private RectTransform target;
        [SerializeField] private LeaderboardEntry prefab;

        private bool done;
        private List<ResponseData> results;

        private void Start() {
            new Thread(async () => {
                await QueryAWSAsync();
            }).Start();
        }

        private void Update() {
            if (!done) return;
            
            done = false;
            Build();
        }

        private void Build() {
            while (target.childCount > 0) {
                var child = target.GetChild(0);
                child.SetParent(null);
                Destroy(child.gameObject);
            }

            for (var i = 0; i < results.Count; i++) {
                var responseData = results[i];
                var entry = Instantiate(prefab, target);
                entry.Build(i+1, responseData.Score, responseData.PlayerName);
            }
        }

        private async Task QueryAWSAsync() {
            using var httpClient = new HttpClient();
            var uri = new Uri("https://edgjs1jvk1.execute-api.eu-central-1.amazonaws.com/default/query-scoreboard?count=5");
            var response = await httpClient.GetAsync(uri);
        
            Debug.Log($"[{response.StatusCode}] {response.ReasonPhrase}");
            var json = await response.Content.ReadAsStringAsync();
            var objs = JsonConvert.DeserializeObject<ResponseData[]>(json);

            results = new List<ResponseData>(objs);
            done = true;
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
}