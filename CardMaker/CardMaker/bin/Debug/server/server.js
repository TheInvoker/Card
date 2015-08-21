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
			_masterArray.forEach(function(_templateListObj) {
				
				var Temp_templateList = [];
				_templateListObj.angles.forEach(function(_templateObj) {
					if (_templateObj.active) {
						Temp_templateList.push(_templateObj);
					}
				});
				
				if (Temp_templateList.length > 0) {
					_templateListObj.angles = Temp_templateList;
					Temp_masterArray.push(_templateListObj);
				}
			});
			
			masterArray = Temp_masterArray;
		}
	});
}, 1000);


var clientIDObject = {};
var resultObject = {};

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
	var i, l = masterArray.length, s = 12;
	var result = [];
	for(i=page*s; i<Math.min(l, (page+1)*s); i+=1) {
		var templateObj = masterArray[i].angles[0];
		result.push({
			"id" : masterArray[i].id,
			"title" : masterArray[i].title,
			"description" : masterArray[i].description,
			"url" : templateObj.template
		});
	}
	return result;
}

function getCreatedLink(templateName, newFilename) {
	var i,j,k,l = masterArray.length;
	for(i=0; i<l; i+=1) {
		var llistobj = masterArray[i];
		
		if (llistobj.id == templateName) {
			var llist = llistobj.angles;
			k = llist.length;
			for(j=0;j<k;j+=1) {				
				var newLink = llist[j].result + newFilename;
				if (fs.existsSync(newLink)) {
					return newLink;
				}
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
	
	//console.log("hi: " + req.url);
	
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
				var clientID = fields["clientID"][0];
				var templateName = fields["templateName"][0];
				var modTime = fields["modTime"][0];
				
				var file = null;
				if (files["fileToUpload"] && files["fileToUpload"].length > 0) {
					file = files["fileToUpload"][0];
					
					console.log("Detected image: " + file.path);
					
					if (clientID in clientIDObject) {
						clientIDObject[clientID].push([file.path, modTime]);
					} else {
						clientIDObject[clientID] = [[file.path, modTime]];
					}
				} else {
					console.log("Trying to use saved image with mod time: " + modTime);

					if (clientID in clientIDObject) {
						var arr = clientIDObject[clientID];
						for(var i=0;i<arr.length;i+=1) {
							if (arr[i][1] == modTime) {
								var savedPath = arr[i][0];
								if (fs.existsSync(savedPath)) {
									console.log("Found the image");
									file = {
										"path" : savedPath
									}
								}
								break;
							}
						}
					}
				}

				if (file == null) {
					WriteHeaderMode('image/png', res, 200);
					res.end("", 'binary');
				} else {
				
					
					if (clientID in resultObject && templateName in resultObject[clientID] && modTime in resultObject[clientID][templateName]) {
						var createdlink = resultObject[clientID][templateName][modTime];
						if (fs.existsSync(createdlink)) {
							fs.readFile(createdlink, function(err, data) {
								if (err) {
									console.log("coldn't read");
									WriteHeaderMode('image/png', res, 200);
									res.end("", 'binary');
								} else {
									console.log("found generated image");
									WriteHeaderMode('image/png', res, 200);
									res.end(data, 'binary');
								}
							});
						} else {
							WriteHeaderMode('image/png', res, 200);
							res.end("", 'binary');
						}
					} else {
					
					
						var newFilename = uuid.v4()+'.png';
						var child = spawn('java', ['-cp', 'java-json.jar:.', 'PlutoMake', file.path, newFilename, templateName]);
						
						child.on('close', function (exitCode) {
							//fs.unlink(file.path, function(){
							//});
							
							if (exitCode === 0) {
								console.log("finished with code: " + exitCode);
								
								var createdlink = getCreatedLink(templateName, newFilename);
								
								var a, b;
								if (clientID in resultObject) {
									a = resultObject[clientID];
								} else {
									resultObject[clientID] = {};
									a = resultObject[clientID];
								}
								if (templateName in a) {
									b = a[templateName];
								} else {
									a[templateName] = {};
									b = a[templateName];
								}
								b[modTime] = createdlink;
								//console.log(JSON.stringify(resultObject));
								
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
					}
				}
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