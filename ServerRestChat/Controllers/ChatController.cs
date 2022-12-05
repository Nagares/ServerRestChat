using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerRestChat.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace ServerRestChat.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        static Dictionary<string, List<ChatData>> chat = new Dictionary<string, List<ChatData>>();

        [HttpGet]
        public ActionResult Get()
        {
            return StatusCode(201);
        }

        //

        [HttpGet("{Name}")]
        public ActionResult GetSync(string Name)
        {
            List<ChatData> data = new List<ChatData>(chat[Name]);
            chat[Name].Clear();
            return StatusCode(201, data);

        }


        //here it is decided how the server will process the received object
        [HttpPost]
        public ActionResult Dialog(ChatData cdata)
        {
            switch (cdata.Action)
            {
                case "Login":
                    return StatusCode(DoLogin(cdata), cdata);

                case "Logout":
                    return StatusCode(DoLogout(cdata));

                case "Message":
                    StatusCode(DoMessage(cdata));
                    break;
            }

            return null;
        }


        //For each user, responses are generated containing the incoming message
        private int DoMessage(ChatData cdata)
        {
            if (string.IsNullOrEmpty(cdata.ToName))
            {
                foreach (KeyValuePair<string, List<ChatData>> item in chat)
                {
                    item.Value.Add(cdata);
                }

            }//common message
            else
            {
                if (!chat.ContainsKey(cdata.ToName)) { return 400; }
                chat[cdata.ToName].Add(cdata);


            }//privat message
            return 201;
        }

        //server Remove users
        private int DoLogout(ChatData cdata)
        {
            if (!chat.ContainsKey(cdata.FromName))
            {
                return 401;
            }

            
            return ListToRefresh(cdata, " left chat!");
        }

        //server registers users
        private int DoLogin(ChatData cdata)
        {
            //check uniquness of the name
            if (chat.ContainsKey(cdata.FromName))
            {
                return 401;
            }
            
            return ListToRefresh(cdata, " joined chat!");
        }

        //generates a response containing information for usernames that it is necessary to update the list of users,
        //also transfers List users names
        //add new user to dictionary
        //Remove user from dictionary
        private int ListToRefresh(ChatData cdata, string message)
        {

            cdata.Message = cdata.FromName + message; 
            
            //add new user to dictionary
            if(message == " joined chat!")
            chat.Add(cdata.FromName, new List<ChatData>());
            else chat.Remove(cdata.FromName); //Remove user from dictionary

            List<string> users = chat.Keys.ToList<string>();
            string names = JsonSerializer.Serialize(users);
            cdata.Data = names;

            ChatData rew = new ChatData()
            {
                FromName = cdata.FromName,
                ToName = string.Empty,
                Message = string.Empty,
                Data = names,
                FileName = string.Empty,
                Action = "Refresh"
            };

            foreach (KeyValuePair<string, List<ChatData>> item in chat)
            {
                item.Value.Add(cdata);
                item.Value.Add(rew);
            }

            return 201;
        }


    }
}
