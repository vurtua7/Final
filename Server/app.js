var io = require('socket.io')(process.envPort||3000);
var shortid = require('shortid');
var MongoClient = require('mongodb').MongoClient;
var url = "mongodb://localhost:27017/";

console.log("Server Started");

var questions = [];
var dbObj;

MongoClient.connect(url, (err, client) =>
{
    if(err)throw err;

    dbObj = client.db("SocketGameData");
});

io.on('connection', (socket) =>{
    
	socket.on('disconnect', () =>{
		console.log("We Dead");
    });

    socket.on('load data', (data) =>
    {
        dbObj.collection("playerData").find({}).toArray((err, result) =>
        {
            if (err) throw err;

            console.log("Sent data to client. Data: ");
            console.log(JSON.stringify(result, null, 4) + "\n");

            socket.broadcast.emit('receiveServerData', result[0]);
            socket.emit('receiveServerData', result[0]);
        });
    });

	socket.on('send data', (data) =>{
        console.log(JSON.stringify(data, null, 4));
        dbObj.collection("playerData").save(data, (err, res) =>
        {
            if(err){
                console.log("Go kill yourslef!");
                throw err;
            }
            console.log("data saved to MongoDB");
        });
    });
});