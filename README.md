
Explica el mecanismo de Registro / Login utilizado (máximo 5 líneas)


Se le pide al usuario un nombre de usuario y una contraseña, esta ultima se convierte en una secuencia de bytes y se cifra usando un SALT ( aleatorio) y SHA512. En el login se aplica lo mismo, se compara con el Hash almacenado y si coinciden el acceso es concedido.

Realiza una pequeña explicación de cada uno de los pasos que has hecho especificando el procedimiento que empleas en cada uno de ellos. 


El texto se pasa a bytes y se firma usando la clave privada del emisor.
Se cifra el mensaje con la clase ClaveSimetrica y se guarda como TextoCifrado.
Se extraen la Key y el IV usados para cifrar y se cifran con la clave publica del receptor
El receptor descifra la clave simétrica (clave e IV) usando su clave privada y el método DescifrarMensaje()
Se recupera la clave simétrica y es entonces que se descfria el texto, finalmente ell receptor verifica que la firma coincida con el texto descifrado usando ComprobarFirma(), empleando la clave pública del emisor. Esto valida que el mensaje no fue modificado.


Una vez realizada la práctica, ¿crees que alguno de los métodos programado en la clase asimétrica se podría eliminar por carecer de una utilidad real?

En teoria Sì, se podría eliminar el método FirmarMensaje(byte[] MensajeBytes, RSAParameters ClavePublicaExterna). Ya que la  la firma digital se realiza siempre con la clave privada del emisor, no con una clave pública externa. Por lo tanto este metodo no aporta tanto al contexto de seguridad que estamos trabajando
