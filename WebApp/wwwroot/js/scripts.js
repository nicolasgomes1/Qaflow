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
}

