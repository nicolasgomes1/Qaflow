//function updateTimeElapsed(timeElapsed) {
//    var timeElapsedLabel = document.getElementById('timeElapsedLabel');
//    timeElapsedLabel.innerText = timeElapsed;
//}

function saveAsFile(filename, fileData) {
    // Create a download link
    var element = document.createElement('a');
    element.setAttribute('href', fileData);
    element.setAttribute('download', filename);

    // Append the link to the document
    document.body.appendChild(element);

    // Programmatically click the link to trigger the download
    element.click();

    // Remove the link from the document
    document.body.removeChild(element);

    console.log("arrived");
};

//function saveAsFile(filename, bytesBase64) {
//    var link = document.createElement('a');
//    link.href = bytesBase64;
//    link.download = filename;
//    link.click();
//}