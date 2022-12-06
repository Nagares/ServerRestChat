# ServerRestChat
---

This application is a **webAPI**, a server for exchanging messages like a chat. All chat participants are pre-registered on the site.

---
##### Used Language: 
- **_C#_**
---
##### Platforms that were used: 
- **_dotnet_\packs\Microsoft.AspNetCore.App.Ref\3.1.10**
- **_dotnet_\packs\Microsoft.NETCore.App.Ref\3.1.0**
---

The server will contain a *Dictionary* of usernames that are keys and a ```List<ChatData>```.
##### **ChatData** is a special data object for client-server exchange, which contains:
- ```FromName``` - sender name;
- ```ToName``` - name of the recipient (if the message is private),
- ```Message``` - text message;
- ```Data``` - file attached to the message;
- ```FileName``` - file name;
- ```Action``` - action to be taken with the received object.

---

##### The server receives a **POST request**, and then decides how to process the received information:
- **_Login_** - ```DoLogin()``` function. Check, if the username is already existing on the server, if it does not, then it calls the ```ListToRefresh()``` function to add the user to the server.
- **_Logout_** - ```DoLogout()``` function. Check, if the username exist,if it does, then calls the ```ListToRefresh()``` function to delete the user from the server.
- ```ListToRefresh()``` function. Add || Remove users from the *Dictionary* (users will be notified about it). Generates a response for the users that it is **necessary** for them to update the list of users. Also sends an updated list of usernames.
- **_Message_** - ```DoMessage()``` function. Responses for each user, which containing an incoming message, are generated. This function will also generate an private messages.
- ```GetSync()``` - main function. Will pass **_all previously created_** message-objects to the client side. When a client generates an automatic ```HttpGet("{Name}")``` request including the client's **(user's) name**, afterwords it receives a response from the server ```List<ChatData>``` (all messages sent to the server since the last request).
---


