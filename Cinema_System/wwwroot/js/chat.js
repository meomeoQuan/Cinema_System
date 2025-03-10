"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Disable the send button until the connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.textContent = user + " says: " + message;
});

// Start the connection
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    console.error("Connection failed: " + err.toString());
});

// Send message to the Hub
document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;

    if (!user || !message) {
        alert("User and Message cannot be empty!");
        return;
    }

    connection.invoke("SendMessage", user, message).catch(function (err) {
        console.error("Send failed: " + err.toString());
    });

    event.preventDefault();
});
