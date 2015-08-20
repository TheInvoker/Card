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
			
			var Temp_masterArray = [];
			_masterArray.forEach(function(_templateList) {
				
				var Temp_templateList = [];
				_templateList.forEach(function(_templateObj) {
					if (_templateObj.active) {
						Temp_templateList.push(_templateObj);
					}
				});
				if (Temp_templateList.length > 0) {
					Temp_masterArray.push(Temp_templateList);
				}
			});
			
			masterArray = Temp_masterArray;
		}
	});
}, 1000);


var html = "";
fs.readFile('./index.html', function (err, data) {
	if (err) {
		console.log("Error reading html file...");
	} else {
		html = data.toString();
	}
});
 
 
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
		result.push(masterArray[i][0].template);
	}
	return result;
}

function getCreatedLink(templateName, newFilename) {
	var i,j,k,l = masterArray.length;
	for(i=0; i<l; i+=1) {
		var llist = masterArray[i];
		k = llist.length;
		for(j=0;j<k;j+=1) {
			var curTemplateName = llist[j].template.split("/")[1];
			if (curTemplateName != templateName) {
				continue;
			}
			
			var newLink = llist[j].result + newFilename;
			if (fs.existsSync(newLink)) {
				return newLink;
			}
		}
	}
	return null;
}

function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}


var PORT = 41302;
http.createServer(function (req, res) {
    // get the array of parameters
	var paramsArray = GetURLSegments(req);
	var segmentLength = paramsArray.length;
	
	console.log("hi: " + req.url);
	
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
							res.end("[]");
						} else {
							WriteHeaderMode('image/png', res, 200);
							res.end(data, 'binary');
						}
					});
				} else {
					WriteHeaderMode('text/html', res, 200);
					res.end("[]");
				}
			}
		} else if (paramsArray[0] == "process") {
            var form = new multiparty.Form();
            form.parse(req, function(err, fields, files) {
				var templateName = fields["templateName"][0];
				var file = files["fileToUpload"][0];

				console.log("Detected image: " + file.path);
				
				var newFilename = uuid.v4()+'.png';
				var child = spawn('java', ['-cp', 'java-json.jar:.', 'PlutoMake', file.path, newFilename, templateName]);
				
				child.on('close', function (exitCode) {
					fs.unlink(file.path, function(){
					});
					
					if (exitCode === 0) {
						console.log("finished with code: " + exitCode);
						
						var createdlink = getCreatedLink(templateName, newFilename);
						
						if (createdlink != null)  {
							fs.readFile(createdlink, function(err, data) {
								if (err) {
									console.log("coldn't read");
									WriteHeaderMode('text/html', res, 200);
									res.end("[]");
								} else {
									WriteHeaderMode('image/png', res, 200);
									res.end(data, 'binary');
								}
							});
						} else {
							console.log("created file not found");
							WriteHeaderMode('text/html', res, 200);
							res.end("[]");
						}

					} else {	
						console.error('Something went wrong!');
						
						WriteHeaderMode('text/html', res, 200);
						res.end("[]");
					}
				});

				// If you’re really just passing it through, though, pass {stdio: 'inherit'}
				// to child_process.spawn instead.
				child.stderr.on('data', function (data) {
					fs.unlink(file.path, function(){
					});
					
					process.stderr.write(data);
					
					WriteHeaderMode('text/html', res, 200);
					res.end("[]");
				});
			
            });
            form.on('error', function(err) {
				console.log(JSON.stringify(err));
				WriteHeaderMode('text/html', res, 200);
				res.end("[]");
            });
		} else {
			WriteHeaderMode('text/html', res, 200);
			res.end("[]");
		}
	} else {
		// send back the index.html page
		WriteHeaderMode('text/html', res, 200);
		res.end(html);
	}
}).listen(PORT);

//console.log('Server running at http://127.0.0.1:' + PORT + '/');
console.log('Server running at cms-chorus.utsc.utoronto.ca:' + PORT + '/');