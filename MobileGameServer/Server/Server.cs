using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;


namespace MobileGameServer.Server
{

    public static class Server
    {
        private static List<AsyncOperation> asyncOperations;

        public static string Prefix { get { return "http://127.0.0.1:2000/"; } }
        public static string ScoresFilePath { get { return "../../SavedScores/Scores.txt"; } }

        public static void AddOperation(AsyncOperation op)
        {
            asyncOperations.Add(op);
        }
        public static void RemoveOperation(AsyncOperation op)
        {
            asyncOperations.Remove(op);
        }

        /// <summary>
        /// The key represent the associated device id
        /// </summary>
        private static Dictionary<string,Score> scores;

        public static void AddScore(Score s)
        {
            if (scores.ContainsKey(s.DeviceID))
                scores[s.DeviceID] = s;
            else
                scores.Add(s.DeviceID, s);


            if (!File.Exists(ScoresFilePath))
            {
                FileStream file = File.Create(ScoresFilePath);
                file.Close();
            }

            string[] allLines = File.ReadAllLines(ScoresFilePath);
            string newJsonString = JsonConvert.SerializeObject(s);
            string newLine = string.Format("{0}-{1}", s.DeviceID, newJsonString);

            bool idAlredyExist = false;
            if (allLines.Length <= 0) //file empty
            {
                allLines = new string[] { newLine };
            }
            else
            {
                for (int i = 0; i < allLines.Length; i++)
                {
                    if (allLines[i].Split('-')[0] == s.DeviceID.ToString())
                    {
                        allLines[i] = newLine;
                        idAlredyExist = true;
                    }
                }
                if (!idAlredyExist) //is score for new deviceID?
                {
                    allLines = allLines.Concat(new string[] { newLine }).ToArray();
                }
            }
            File.WriteAllLines(ScoresFilePath,allLines);
        }
        public static Score GetScore(string deviceID)
        {
            if (scores.ContainsKey(deviceID))
                return scores[deviceID];
            return null;
        }
        /// <summary>
        /// Return true if score is overwritten
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="newScore"></param>
        /// <returns></returns>
        public static bool SetScore(string deviceID,Score newScore)
        {
            Score currentScore = GetScore(deviceID);
            if(currentScore != null)
            {
                if(newScore.Points > currentScore.Points)
                {
                    //set new score
                    scores[deviceID] = newScore;
                    return true;
                }
            }
            return false;
        }
        public static Dictionary<string,Score> Scores
        {
            get
            {
                return scores;
            }
        }
        public static void Init()
        {
            scores = new Dictionary<string, Score>();
            asyncOperations = new List<AsyncOperation>();

            Score newScore = new Score();
            newScore.Date = DateTime.Now;
            newScore.Device = "mattia android";
            newScore.DeviceID = "fb7a316abdea11556fcb35de77b602429139c8c5";
            newScore.EndPoint = null;
            newScore.Points = 100;
            AddScore(newScore);

            Score newScore2 = new Score();
            newScore2.Date = DateTime.Now;
            newScore2.Device = "mattia android";
            newScore2.DeviceID = "2";
            newScore2.EndPoint = null;
            newScore2.Points = 12;
            AddScore(newScore2);


            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url/*"http://www.google.com/#q=ciao+come+stai"*/);
            new AsyncOperation();
        }


        public static void Update()
        {
            for (int i = 0; i < asyncOperations.Count; i++)
            {
                asyncOperations[i].Update();
            }
        }
    }
}
