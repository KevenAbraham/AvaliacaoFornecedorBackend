document.addEventListener('DOMContentLoaded', function () {
    // Array para armazenar os documentos selecionados
    let documentos = [];

    // Elementos do DOM
    const uploadInput = document.getElementById('upload');
    const fileCounter = document.getElementById('fileCounter');
    const documentList = document.getElementById('documentList');

    // Event listener para quando um arquivo é selecionado
    uploadInput.addEventListener('change', function () {
        // Limpa a lista de documentos ao selecionar novos arquivos
        documentos = [];
        documentList.innerHTML = '';

        // Itera sobre os arquivos selecionados
        for (let i = 0; i < uploadInput.files.length; i++) {
            // Adiciona cada arquivo ao array de documentos
            documentos.push(uploadInput.files[i]);

            // Cria um elemento de lista para exibir o nome do arquivo
            const listItem = document.createElement('div');
            listItem.textContent = uploadInput.files[i].name;
            documentList.appendChild(listItem);
        }

        // Atualiza o contador de arquivos selecionados
        fileCounter.textContent = `Total de arquivos selecionados: ${uploadInput.files.length}`;
    });
});