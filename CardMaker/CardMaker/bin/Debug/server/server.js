var http = require('http')
 , fs = require('fs')
 , request = require('request')
 , qs = require('querystring')
 , multiparty = require('multiparty')
 , spawn = require('child_process').spawn
 , uuid = require('node-uuid'); 
 
 
 
var masterArray = [];
setInterval(function() {
	fs.readFile('master.js', function (err, data) {
		if (err) {
			console.log("Couldn't read from master.js")
		} else {
			var _masterArray = JSON.parse(data);
			masterArray = [];
			
			var filterTrue = [];
			_masterArray.forEach(function(value) {
				if (value.active) {
					masterArray.push(value);
				}
			});
		}
	});
}, 1000);

 
 
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
		result.push(masterArray[i].template);
	}
	return result;
}

function getCreatedLinks(newFilename) {
	var i, l = masterArray.length;
	var result = [];
	for(i=0; i<l; i+=1) {
		var newLink = masterArray[i].result + newFilename;
		if (fs.existsSync(newLink)) {
			result.push(newLink);
		}
	}
	return result;
}

function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}


var PORT = 41302;
http.createServer(function (req, res) {
    // get the array of parameters
	var paramsArray = GetURLSegments(req);
	var segmentLength = paramsArray.length;
	
	if (segmentLength > 0) {
		
		if (segmentLength > 1) {

			if (paramsArray[0] == "template" && !isNaN(paramsArray[1])) {
				var page = parseInt(paramsArray[1], 10);
				var result = JSON.stringify(getTemplateLinks(res, page));
				
				WriteHeaderMode('text/html', res, 200);
				res.end(result);
			} else if (paramsArray[0] == "template" || paramsArray[0] == "result") {
				var requestURL = "." + req.url.toLowerCase();

				if (fs.existsSync(requestURL) && (
					endsWith(requestURL, ".png") ||
					endsWith(requestURL, ".jpg") || 
					endsWith(requestURL, ".jpeg")
					))  {
					fs.readFile(requestURL, function(err, data) {
						if (err) {
							WriteHeaderMode('text/html', res, 200);
							res.end();
						} else {
							WriteHeaderMode('image/png', res, 200);
							res.end(data, 'binary');
						}
					});
				} else {
					WriteHeaderMode('text/html', res, 200);
					res.end();
				}
			}
		} else if (paramsArray[0] == "process") {
            var form = new multiparty.Form();
            form.parse(req);
			
            form.on('file', function(name, file) {
				console.log("Detected image: " + file.path);
				
				
				var newFilename = uuid.v4()+'.png';
				var child = spawn('java', ['-cp', 'java-json.jar:.', 'PlutoMake', file.path, newFilename]);
				
				child.on('close', function (exitCode) {
					fs.unlink(file.path, function(){
					});
					
					if (exitCode === 0) {
						console.log("finished with code: " + exitCode);
						
						var createdList = getCreatedLinks(newFilename);
						
						WriteHeaderMode('text/html', res, 200);
						res.end(JSON.stringify(createdList));
					} else {	
						console.error('Something went wrong!');
						
						WriteHeaderMode('text/html', res, 200);
						res.end();
					}
				});

				// If you’re really just passing it through, though, pass {stdio: 'inherit'}
				// to child_process.spawn instead.
				child.stderr.on('data', function (data) {
					fs.unlink(file.path, function(){
					});
					
					process.stderr.write(data);
					
					WriteHeaderMode('text/html', res, 200);
					res.end();
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

//console.log('Server running at http://127.0.0.1:' + PORT + '/');
console.log('Server running at cms-chorus.utsc.utoronto.ca:' + PORT + '/');