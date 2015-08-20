function sendData(formData) {
    var url = "http://cms-chorus.utsc.utoronto.ca:41302";
	$("#drop input").val("");
	
	formData.append('templateName', "bump1");
	formData.append('clientID', myClientID);
	
	$.ajax({
		type: 'POST',
		url: url + "/process",
		data: formData,
		dataType: 'binary',
		responseType:'arraybuffer',
		contentType:false,
		cache:false,
		processData:false,
		timeout: 30 * 1000,
		beforeSend: function( xhr ) {
			$("div.row.all-previews").html("<img class=\"all-previews-loader\" src='images/icons/loader.gif'></img>");
		},
		success: function(blob,status,xhr) {
            $("div.row.all-previews").html("<div class='col-md-8 preview'><img src='data:image/png;base64," + _arrayBufferToBase64(blob) + "'/></div>");
		},
		error: function(data,status,xhr) {
			alert(xhr);
		}
	});
}

$(document).ready(function() {
	// use this transport for "binary" data type
	$.ajaxTransport("+binary", function(options, originalOptions, jqXHR) {
		// check for conditions and support for blob / arraybuffer response type
		if (window.FormData && ((options.dataType && (options.dataType == 'binary')) || (options.data && ((window.ArrayBuffer && options.data instanceof ArrayBuffer) || (window.Blob && options.data instanceof Blob))))) {
			return {
				// create new XMLHttpRequest
				send: function(headers, callback) {
					// setup all variables
					var xhr = new XMLHttpRequest(),
						url = options.url,
						type = options.type,
						async = options.async || true,
						// blob or arraybuffer. Default is blob
						dataType = options.responseType || "blob",
						data = options.data || null;

					xhr.addEventListener('load', function() {
						var data = {};
						data[options.dataType] = xhr.response;
						// make callback and send data
						callback(xhr.status, xhr.statusText, data, xhr.getAllResponseHeaders());
					});

					xhr.open(type, url, async);

					// setup custom headers
					for (var i in headers) {
						xhr.setRequestHeader(i, headers[i]);
					}

					xhr.responseType = dataType;
					xhr.send(data);
				},
				abort: function() {
					jqXHR.abort();
				}
			};
		}
	});

    $("#upload").on('submit', function() {
        var formData = new FormData(this);
        sendData(formData);
        return false;
    });
    
    var doc = document.getElementById("drop");
    doc.ondragover = function () { 
        this.className = 'hover'; 
        return false; 
    };
    $(doc).on('dragleave dragstop drop', function() {
        this.className = ''; 
        return false; 
    });
    doc.ondrop = function (event) {
        event.preventDefault && event.preventDefault();

        var files = event.dataTransfer.files;
        var formImage = new FormData();
        formImage.append('fileToUpload', files[0]);
        sendData(formImage);

        return false;
    };
    
    $('#drop a').click(function(){
        // Simulate a click on the file input button
        // to show the file browser dialog
        $(this).next('input').click();
    });
    $("#drop input").on('change', function () {
		if ($(this).val().trim() != "") {
			$("#upload").submit();
	    }
    });
});

// http://stackoverflow.com/a/9458996/128597
function _arrayBufferToBase64(buffer) {
	var binary = '';
	var bytes = new Uint8Array(buffer);
	var len = bytes.byteLength;
	for (var i = 0; i < len; i++) {
		binary += String.fromCharCode(bytes[i]);
	}
	return window.btoa(binary);
};

function getClientID() {
	return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
		var r = Math.random()*16|0,v=c=='x'?r:r&0x3|0x8;return v.toString(16);
	});
}

var myClientID = getClientID();