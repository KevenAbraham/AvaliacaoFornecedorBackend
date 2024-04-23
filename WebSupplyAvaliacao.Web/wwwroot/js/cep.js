$(document).ready(function () {
    $('#buscarCEP').click(function () {
        var cep = $('#cep').val();
        var url = 'https://servicos.remaza.com.br/api/CEP/' + cep;

        $.get(url, function (data) {
            if (data.ERRO) {
                $('#erroCep').text("CEP invalido.");
                $('#erroCep').show();
            } else {
                $('#erroCep').empty();
                $('#numero').val("");
                $('#complemento').val("");
                $('#logradouro').val(data.Logradouro);
                $('#bairro').val(data.Bairro);
                $('#cidade').val(data.Cidade);
                $('#uf').val(data.Estado);
            }
        }).fail(function () {
            $('#erroCep').text("Falha ao buscar CEP");
        });
    });
});