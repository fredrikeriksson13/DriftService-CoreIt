$(function () {
    $("#datepicker").datepicker({
        dateFormat: 'yy-mm-dd',
        //autoclose: true
    });
});

$('#TbMessage').keyup(function () {
    var max = 98; //max 160, -2 mellan rubrik och meddelande, signatur -11 insatt för CoreIT, rubrik -50.
    var message = $(this).val().length;
    if (message >= max) {
        $('#divCounter span').text('Above text will use more then one SMS');
    } else {
        $('#divCounter span').text('');
    }
});


