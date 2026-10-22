// Validação básica dos formulários sem dependências externas.
// As validações do servidor continuam sendo a verificação principal.
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('form[novalidate]').forEach(function (form) {
        var campos = Array.from(form.querySelectorAll('[data-val="true"]'));

        function validar(campo) {
            var valor = campo.value.trim();
            var mensagem = '';
            var minimo = campo.getAttribute('data-val-minlength-min');
            var maximo = campo.getAttribute('data-val-length-max');
            var intervaloMin = campo.getAttribute('data-val-range-min');
            var intervaloMax = campo.getAttribute('data-val-range-max');

            if (!valor && campo.hasAttribute('data-val-required')) {
                mensagem = campo.getAttribute('data-val-required') || '';
            } else if (minimo !== null && valor.length < Number(minimo)) {
                mensagem = campo.getAttribute('data-val-minlength') || '';
            } else if (maximo !== null && valor.length > Number(maximo)) {
                mensagem = campo.getAttribute('data-val-length') || '';
            } else if (intervaloMin !== null &&
                (valor === '' || !Number.isFinite(Number(valor)) ||
                    Number(valor) < Number(intervaloMin) || Number(valor) > Number(intervaloMax))) {
                mensagem = campo.getAttribute('data-val-range') || '';
            }

            var aviso = Array.from(form.querySelectorAll('[data-valmsg-for]'))
                .find(function (elemento) { return elemento.getAttribute('data-valmsg-for') === campo.name; });

            if (aviso) aviso.textContent = mensagem;
            campo.classList.toggle('invalido', mensagem !== '');
            return mensagem === '';
        }

        campos.forEach(function (campo) {
            campo.addEventListener('input', function () { validar(campo); });
            campo.addEventListener('change', function () { validar(campo); });
        });

        form.addEventListener('submit', function (evento) {
            var primeiroInvalido = null;
            campos.forEach(function (campo) {
                if (!validar(campo) && !primeiroInvalido) primeiroInvalido = campo;
            });
            if (primeiroInvalido) {
                evento.preventDefault();
                primeiroInvalido.focus();
            }
        });
    });
});
