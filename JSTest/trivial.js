var port = process.argv[2]
var http = require("http");
http.createServer(function(request, response) {
    handler(response);
}).listen(port);

function handler(response)
{
    // The setTimeout call here is to simulate some work happening.
    setTimeout(() => {
        response.writeHead(200, { "Content-Type": "text/plain" });
        response.write("");
        response.end();
    }, 100);
}