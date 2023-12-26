// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function CopyFunction(codeID) {
    /* Textfeld Get the text field */
    var copyText = document.getElementById(codeID);

    /* Select the text field */
    copyText.select();
    copyText.setSelectionRange(0, 99999); /* Für Mobile Devices */

    /* Kopiere den Text us dem Textfeld */
    document.execCommand("copy");

    /* Hinweis, wenn Text kopiert wurde */
    alert("In Zwischenablage kopiert");
}