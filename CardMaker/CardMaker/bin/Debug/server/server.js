var http = require('http')
 , fs = require('fs')
 , request = require('request')
 , qs = require('querystring');
 
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

var controller = {
	
	// Gets clips and their votes from the database 
	getClips: function(res, connection) {
		res.end("");
	}
};

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

  
// Writes the header in the response
function WriteHeaderMode(id, response, code) {
    response.writeHead(code, {
        'Content-Type': id,
        'Access-Control-Allow-Origin': "*"
    });
}

/*   Takes a request object, and returns an array of segments used in the url. For example if the url 
 *   property in the request object was http://localhost:41302/hi/hi/hi?h=1, then it would return
 *   ["hi","hi","hi?h=1"].
 */
function GetURLSegments(request) {
    var segments = [];
    var rawsegments = request.url.split("/");
    var rawsegments_len = rawsegments.length;
    for (var i = 0; i < rawsegments_len; i += 1) {
        if (rawsegments[i] != "") {
            segments.push(rawsegments[i].toLowerCase());
        }
    }
    return segments;
}

function getURLParameter(link, name) {
    return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(link)||[,""])[1].replace(/\+/g, '%20'))||null;
}

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



var PORT = 41302;
http.createServer(function (req, res) {
    // get the array of parameters
	var paramsArray = GetURLSegments(req);
	var segmentLength = paramsArray.length;

	if (segmentLength > 0) {
		WriteHeaderMode('text/html', res, 200);
		
		// if user wanted clips
		if (segmentLength == 2 && paramsArray[0] == "template") {
			var page = parseInt(paramsArray[1], 10);
			if (isNaN(page) || page == null || page < 0) page = 0;
			

			res.end();
		} else {
			res.end();
		}
		
	} else {
		// send back the index.html page
		WriteHeaderMode('text/html', res, 200);
		fs.readFile('./index.html', function (err, data) {
			if (err) res.end();
			res.end(data);
		});
	}
}).listen(PORT);

console.log('Server running at http://127.0.0.1:' + PORT + '/');