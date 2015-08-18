function sendData(formData) {
    var url = "http://cms-chorus.utsc.utoronto.ca:41302";
	
	$.ajax({
        type: 'POST',
        url: url + "/process",
        data: formData,
        dataType: 'json',
        contentType:false,
        cache:false,
        processData:false,
        timeout: 30 * 1000,
        beforeSend: function( xhr ) {
            $("div.row.all-previews").html("Please wait while I get your images. If you have read this part by now then you are pretty fast at reading.");
        },
        success: function(jsonData,status,xhr) {
            var str = "";
            for(var i=0; i<jsonData.length; i+=1) {
				str += "<div class='col-md-8 preview'><img src='"+url+"/"+jsonData[i]+"'></img></div>";
            }
            $("div.row.all-previews").html(str);
        },
        error: function(data,status,xhr) {
            alert(xhr);
        }
    });
}

$(document).ready(function() {
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
     $("#drop input").change(function (){
       $("#upload").submit();
     });
});