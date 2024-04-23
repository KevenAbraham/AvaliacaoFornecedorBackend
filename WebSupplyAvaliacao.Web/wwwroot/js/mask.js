$(document).ready(function () {
    // Máscara para CNPJ
    $('#cnpj').mask('00.000.000/0000-00');
    $('.cnpj').mask('00.000.000/0000-00');
    // Máscara para CEP
    $('#cep').mask('00000-000');
    // Máscara para Telefone
    $('#telefone').mask('(00) 00000-0000');

    $('form').submit(function () {
        $('#cnpj').unmask();
        $('#cep').unmask();
        $('#telefone').unmask();
    });
});