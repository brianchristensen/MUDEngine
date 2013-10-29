var scroll = true;

$(function () {
    // Create proxy
    var mud = $.connection.gameHub;

    // If enter is pressed in the command textbox
    $("#tbCommand").keyup(function (e) {
        // Click the hidden button
        if (e.keyCode == 13) {
            $("#btnInput").click();
        }
    });

    // On enter pressed
    $("#btnInput").click(function () {
        // Send command to the server
        mud.server.send($('#tbCommand').val());
        // Clear the msg textbox
        $("#tbCommand").val("");
    });

    // Receive server object data
    mud.client.output = function (obj) {
        // Convert the JSON string into a JSON object
        var data = JSON.parse(obj);        
        // Display object data
        $('#mudList').append('<li>' + data.text + '</li>');
        // Scroll to the bottom of the mud window if mouse is not clicked inside mudWindow
        if (scroll) {
            $("#mudWindow").animate({ scrollTop: $("#mudWindow").prop("scrollHeight") - $('#mudWindow').height() }, 50);
        }
    };

    // Receive server text output
    mud.client.stdout = function (message) {
        // Display text output
        $('#mudList').append('<li>' + message + '</li>');
        // Scroll to the bottom of the mud window
        $("#mudWindow").animate({ scrollTop: $("#mudWindow").prop("scrollHeight") - $('#mudWindow').height() }, 50);
    }

    // Start the connection 
    $.connection.hub.start(function () {
        // Client join game world
        mud.server.join();
        // Set focus on command textbox
        $("#tbCommand").focus();
    });
});        

function setGlobalNoScroll() {
    scroll = false;
}

function setGlobalScroll() {
    scroll = true;
}