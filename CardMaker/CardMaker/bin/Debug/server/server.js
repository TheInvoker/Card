var http = require('http')
 , fs = require('fs')
 , request = require('request')
 , qs = require('querystring');
 
var multiparty = require('multiparty');
 
 
 
 
 
var masterArray = [];
setInterval(function() {
	fs.readFile('./../master.js', function (err, data) {
		if (err) {
			console.log("Couldn't read from master.js")
		} else {
			masterArray = JSON.parse(data);
			
			var i, l = masterArray.length;
			for(i=0; i<l; i+=1) {
				var link = "../" + masterArray[i].template;
				var img = fs.readFileSync(link);
				masterArray[i]["_img"] = img;
			}
		}
	});
}, 1000);

 
 
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


function getTemplateLinks(res, page) {
	var i, l = masterArray.length;
	var result = [];
	for(i=page*6; i<(page+1)*6; i+=1) {
		if (i >= l) {
			return result;
		}
		result.push("../" + masterArray[i].template);
	}
	return result;
}

function getImage(src) {
	var i, l = masterArray.length;
	for(i=0; i<l; i+=1) {
		//console.log(masterArray[i].template);
		if ("/" + masterArray[i].template == src) {
			return masterArray[i]["_img"];
		}
	}
}
var uploadDirectory = 'images/';
// http://localhost:41302/template/card1/business-card.jpg
var PORT = 41302;
http.createServer(function (req, res) {
    // get the array of parameters
	var paramsArray = GetURLSegments(req);
	var segmentLength = paramsArray.length;
	
	if (segmentLength > 0) {
		
		if (segmentLength > 1 && paramsArray[0] == "template") {
			if (!isNaN(paramsArray[1])) {
				var page = parseInt(paramsArray[1], 10);
				var result = JSON.stringify(getTemplateLinks(res, page));
				
				WriteHeaderMode('text/html', res, 200);
				res.end(result);
			} else {
				//console.log(req.url);
				var img = getImage(req.url);
				
				WriteHeaderMode('image/png', res, 200);
				res.end(img, 'binary');
			}
		} else if (paramsArray[0] == "process") {
			console.log("hi");

			
            var form = new multiparty.Form();
            form.parse(req);
			
            form.on('file', function(name, file) {
                var saveFilePath = uploadDirectory + file.originalFilename;
                console.log(file.path);
				console.log(saveFilePath);
				fs.rename(file.path, saveFilePath, function(err) {
                    if (err) {
                        // Handle problems with file saving
						console.log("error");
						console.log(JSON.stringify(err));
                        WriteHeaderMode('text/html', res, 200);
						res.end();
                    } else {
                        // Respond to the successful upload with JSON.
                        // Use a location key to specify the path to the saved image resource.
                        // { location : '/your/uploaded/image/file'}
                        var textboxResponse = JSON.stringify({
                            location: saveFilePath
                        });
	
						console.log(textboxResponse);
                        // If your script needs to receive cookies, set images.upload.credentials:true in
                        // the Textbox.io configuration and enable the following two headers.
                        // res.setHeader('Access-Control-Allow-Credentials', 'true');
                        // res.setHeader('P3P', 'CP="There is no P3P policy."');
                        WriteHeaderMode('text/html', res, 200);
						res.end();
                    }
                });
            });
            form.on('error', function(err) {
				console.log(JSON.stringify(err));
				WriteHeaderMode('text/html', res, 200);
				res.end();
            });

			

		} else {
			WriteHeaderMode('text/html', res, 200);
			res.end();
		}
		
	} else {
		// send back the index.html page
		fs.readFile('./index.html', function (err, data) {
			WriteHeaderMode('text/html', res, 200);
			if (err) res.end();
			res.end(data);
		});
	}
}).listen(PORT);

console.log('Server running at http://127.0.0.1:' + PORT + '/');