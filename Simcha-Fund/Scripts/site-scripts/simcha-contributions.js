$(function () {
    $(".contribute").bootstrapSwitch({
        onText: 'Yes',
        offText: 'No'
    });

    $("#search").on('keyup', function () {
        const text = $(this).val();
        $("table tr:gt(0)").each(function () {
            const tr = $(this);
            const name = tr.find('td:eq(1)').text();
            if (name.toLowerCase().indexOf(text.toLowerCase()) !== -1) {
                tr.show();
            } else {
                tr.hide();
            }
        });
    });
    $("#clear").on('click', function () {
        $("#search").val('');
        $("tr").show();
    });
});