var http = require('http')
 , fs = require('fs')
 , multiparty = require('multiparty')
 , spawn = require('child_process').spawn
 , uuid = require('node-uuid'); 
 

// variables
var html = "";
var clientIDObject = {};
var resultObject = {};
var masterArray = [];

// paths
var MASTER_PATH = 'master.js';
var HOME_PATH = 'index.html';

// helper functions
function log(str) {
	console.log(str);
} 
function cleanInActiveTeplates(_masterArray) {
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
	
	return Temp_masterArray;
} 
function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}
function readMaster() {
	//log("About to read master.js");
	fs.exists(MASTER_PATH, function(exists) {
		if (exists) {
			fs.readFile(MASTER_PATH, function (err, data) {
				if (err) {
					log("Couldn't read from master.js");
				} else {
					var _masterArray;
					try {
						_masterArray = JSON.parse(data);
					} catch(err) {
						_masterArray = [];
						log("JSON error when parsing master.js");
					}
					masterArray = cleanInActiveTeplates(_masterArray);
				}
			});
		} else {
			log("master.js does not exist")
		}
	});
}
function cleanOldFiles() {
	log("cleaning old files");
	
	var i,j,k,l = masterArray.length;
	for(i=0; i<l; i+=1) {
		var llistobj = masterArray[i];
		var llist = llistobj.angles;
		k = llist.length;
		for(j=0;j<k;j+=1) {
			var resultDir = llist[j].result;
			fs.readdir(resultDir, function(err, files) {
				if (err) {
					
				} 
				else {
					files.forEach(function(file, index) {
						var filePath = "./" + resultDir + file;

						fs.exists(filePath, function(exists) {
							if (exists) {
								fs.stat(filePath, function(err, stat) {
									if (err) {
										log(err);
									} 
									else {
										var endTime, now;
										now = new Date().getTime();
										endTime = new Date(stat.ctime).getTime() + 600000;
										
										if (now > endTime) {
											fs.unlink(filePath, function(){
												//log("deleted file: '" + filePath + "'");
											});
										}
									}
								});
							}
						});
					});
				}
			});
		}
	}
}

setInterval(function() {
	readMaster();
}, 1000 * 60);
readMaster();

setInterval(function() {
	cleanOldFiles();
}, 1000 * 60 * 10);
cleanOldFiles();

fs.exists(HOME_PATH, function(exists) {
	if (exists) {
		fs.readFile(HOME_PATH, function (err, data) {
			if (err) {
				log("Error reading index.html file...");
			} else {
				html = data.toString();
			}
		});
	} else {
		log("index.html does not exist")
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


//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

// get first angle of each template in the specified page
function getTemplateLinks(res, page) {
	var i, l = masterArray.length, s = 12;
	var result = [];
	for(i = Math.max(0, page * s); i < Math.min(l, (page + 1) * s); i += 1) {
		var templateObj = masterArray[i].angles[0];
		result.push({
			"id" : masterArray[i].id,
			"title" : masterArray[i].title,
			"description" : masterArray[i].description,
			"url" : templateObj.template,
			"angle_id" : templateObj.id
		});
	}
	return result;
}

function getCreatedLink(templateName, newFilename, angle_id, handler) {
	var i,j,k,l = masterArray.length;
	for(i=0; i<l; i+=1) {
		var llistobj = masterArray[i];
		if (llistobj.id == templateName) {
			var llist = llistobj.angles;
			k = llist.length;
			for(j=0;j<k;j+=1) {
				if (llist[j].id == angle_id) {
					var newLink = llist[j].result + newFilename;
					return handler(newLink);
				}
			}
		}
	}
	handler(null);
}



		
function handleImage(res, file, templateName, clientID, angle_id) {
	var newFilename = uuid.v4()+'.png';
	var child = spawn('java', ['-cp', 'java-json.jar:.', 'PlutoMake', file.path, newFilename, templateName, angle_id]);
	
	child.on('close', function (exitCode) {
		fs.unlink(file.path, function(){
		});
		
		if (exitCode === 0) {
			
			log("finished with code: " + exitCode);
			
			getCreatedLink(templateName, newFilename, angle_id, function(createdlink) {
				
				if (createdlink != null)  {
					fs.exists(createdlink, function(exists) {
						if (exists) {
							
							// cache result link
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
							b[angle_id] = createdlink;
							
							// send back file
							fs.readFile(createdlink, function(err, data) {
								if (err) {
									log("couldn't read generated file");
									
									WriteHeaderMode('image/png', res, 200);
									res.end("", 'binary');
								} else {
									WriteHeaderMode('image/png', res, 200);
									res.end(data, 'binary');
								}
							});
						} 
						
						else {
							log("created file not found: '" + createdlink + "'");
							
							WriteHeaderMode('image/png', res, 200);
							res.end("", 'binary');
						}
					});
				} 
				
				else {
					log("created file not found: '" + createdlink + "'");
			
					WriteHeaderMode('image/png', res, 200);
					res.end("", 'binary');
				}
			});
		} 
		
		else {	
			log('Something went wrong!');
			
			WriteHeaderMode('image/png', res, 200);
			res.end("", 'binary');
		}
	});

	// If you’re really just passing it through, though, pass {stdio: 'inherit'}
	// to child_process.spawn instead.
	child.stderr.on('data', function (data) {
		fs.unlink(file.path, function(){
		});
		
		process.stderr.write(data);
		
		WriteHeaderMode('image/png', res, 200);
		res.end("", 'binary');
	});
}


var PORT = 41302;
http.createServer(function (req, res) {

    // get the array of parameters
	var paramsArray = GetURLSegments(req);
	var segmentLength = paramsArray.length;
	
	if (segmentLength == 0) {
		WriteHeaderMode('text/html', res, 200);
		res.end(html);
	} else {
		
		// at least 2
		if (segmentLength > 1) {

			if (paramsArray[0] == "template" && !isNaN(paramsArray[1])) {
				var page = parseInt(paramsArray[1], 10);
				var result = JSON.stringify(getTemplateLinks(res, page));
				
				WriteHeaderMode('text/html', res, 200);
				res.end(result);
			} 
			
			else if (paramsArray[0] == "template" || paramsArray[0] == "result") {
				var requestURL = "." + req.url.toLowerCase();

				if (endsWith(requestURL, ".png") || endsWith(requestURL, ".jpg") || endsWith(requestURL, ".jpeg"))  {
					fs.exists(requestURL, function(exists) {
						if (exists) {
							fs.readFile(requestURL, function(err, data) {
								if (err) {
									WriteHeaderMode('image/png', res, 200);
									res.end("", 'binary');
								} 
								
								else {
									WriteHeaderMode('image/png', res, 200);
									res.end(data, 'binary');
								}
							});
						} 
						
						else {
							WriteHeaderMode('image/png', res, 200);
							res.end("", 'binary');
						}
					});
				} 
				
				else {
					WriteHeaderMode('image/png', res, 200);
					res.end("", 'binary');
				}
			}
			
			else {
				WriteHeaderMode('text/html', res, 200);
				res.end();
			}
		}
		
		else if (segmentLength == 1) {
			if (paramsArray[0] == "process") {
				
				var form = new multiparty.Form();
				form.parse(req, function(err, fields, files) {
					
					var clientID = fields["clientID"][0];
					var templateName = fields["templateName"][0];
					var angle_id = fields["angleID"][0];
					
					if (files["fileToUpload"] && files["fileToUpload"].length > 0) {
						var file = files["fileToUpload"][0];
						log("Detected image: " + file.path);
						
						if (clientID in resultObject && templateName in resultObject[clientID] && angle_id in resultObject[clientID][templateName]) {
							var createdlink = resultObject[clientID][templateName][angle_id];
							fs.exists(createdlink, function(exists) {
								if (exists) {
									fs.readFile(createdlink, function(err, data) {
										if (err) {
											handleImage(res, file, templateName, clientID, angle_id);
										} 
										
										else {
											log("found generated file");
											
											WriteHeaderMode('image/png', res, 200);
											res.end(data, 'binary');
										}
									});
								} 
								
								else {
									handleImage(res, file, templateName, clientID, angle_id);
								}
							});
						} 
						
						else {
							handleImage(res, file, templateName, clientID, angle_id);
						}
					}
				});
				
				form.on('error', function(err) {
					log(JSON.stringify(err));
					WriteHeaderMode('text/html', res, 200);
					res.end();
				});
			} 
			
			else {
				WriteHeaderMode('text/html', res, 200);
				res.end();
			}
		}
		
		else {
			WriteHeaderMode('text/html', res, 200);
			res.end();
		}
	}
}).listen(PORT);

//log('Server running at http://127.0.0.1:' + PORT + '/');
log('Server running at cms-chorus.utsc.utoronto.ca:' + PORT + '/');