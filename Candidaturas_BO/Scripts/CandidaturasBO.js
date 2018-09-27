var hash = {
    'xls': 1,
    'xlsx': 1,
};

function checkExtension() {
    var filename = $("#uploadFile").val();
    var re = /\..+$/;
    var ext = filename.slice(filename.lastIndexOf(".") + 1).toLowerCase();
    if (hash[ext]) {
        return true;
    } else {
        alert("Tipo de ficheiro inválido. Por favor seleccione um ficheiro Excel válido.");
        event.preventDefault();
        return false;
    }
}