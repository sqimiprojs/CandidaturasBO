var hash = {
    'xls': 1,
    'xlsx': 1,
};

function checkExtension() {
    var filename = $("#uploadFile").val();
    var file = document.getElementById('uploadFile').files[0];
    var re = /\..+$/;
    var ext = filename.slice(filename.lastIndexOf(".") + 1).toLowerCase();
    if (hash[ext] && checkMimeType(file)) {
        if ($("#FileWarning").is(":visible")) {
            $("#FileWarning").hide();
        }
        return true;
    } else {
        $("#FileWarning").text("Tipo de ficheiro inválido. Por favor seleccione um ficheiro Excel válido.");
        $("#FileWarning").show();
        event.preventDefault();
        return false;
    }
}

function checkMimeType(file) {
    return file.type.match("application / vnd.ms - excel") || file.type.match("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
}