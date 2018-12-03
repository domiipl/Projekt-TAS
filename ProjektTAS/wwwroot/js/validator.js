function validator(){
    $('.ui.form')
        .form({
            fields: {
                login: {
                    identifier: 'login',
                    rules: [{
                        type: 'regExp',
                        value: /^[A-Za-z0-9]+/i,
                        prompt: 'Login musi się składać z co najmniej jednej litery lub cyfry'
                      }]
                },
                email: {
                    identifier: 'email',
                    rules:[{
                        type: 'email',
                        prompt: 'Niepoprawny adres e-mail'
                    }]
                },
                password: {
                    identifier: 'password',
                    rules:[
                        {
                        type: 'minLength[6]',
                        prompt: 'Hasło musi posiadać co najmniej 6 znaków'
                    },
                    {
                        type: 'regExp',
                        value: /^[A-Za-z0-9]+/i,
                        prompt: 'Hasło akceptuje wyłącznie cyfry oraz małe i duże litery'
                      }
                    ]
                }
            }
        },
        {
            onSuccess: Rejestracja()
        });
}