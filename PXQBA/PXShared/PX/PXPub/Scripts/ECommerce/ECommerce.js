$(document).ready(function () {

    $('#domain').change(function () {
        
        alert($("select#domain option:selected").attr("id"));

    });


});
