# ElFraccionista-Videojuego
 
Este videojuego es parte del proyecto ***PLATAFORMA DE GESTIÓN Y ANÁLISIS PARA EL VIDEOJUEGO "EL FRACCIONISTA”*** para optar al título de Ingeniería Civil Informática en la Universidad del Bio Bio.

El proyecto fué desarrollado en **Unity** en su versión ***2021.3.30f1***, se recomienda ejecutarlo en esta misma versión para no causar conflicto de versiones.

## Configuraciones necesarias

- Una vez instalado Unity en su versión recomendada solo es necesario abrir la carpeta *El Fraccionista* desde ***Unity Hub*** (Se instala junto con la versión de Unity)

![](https://github.com/Evitha-CS/ElFraccionista-Videojuego/blob/main/Readme_images/Unity_HUB.png)

![](https://github.com/Evitha-CS/ElFraccionista-Videojuego/blob/main/Readme_images/Inicio_Fraccionista_Unity.png)
*Al abrirse el proyecto se deberá mostrar algo similar a la imagen*

- Luego, se deberá configuar Photon Unity Networking, lo primero será crear una cuenta en su página oficial https://www.photonengine.com
- Ya creada una cuenta se deberá ir a ***Dashboard*** y ir a la sección ***CREATE A NEW APP***
![](https://github.com/Evitha-CS/ElFraccionista-Videojuego/blob/main/Readme_images/Photon_createapp.png)

- Aquí se deberá seleccionar el SDK PUN(Photon Unity Networking), dar un nombre a la aplicación y una descripción opcional, tal como se muestra en la imagen:
![](https://github.com/Evitha-CS/ElFraccionista-Videojuego/blob/main/Readme_images/Photon_createapp2.png)

- Al dar en ***CREATE*** se deberá copiar el ***APP ID*** generado y regresar nuevamente a ***Unity***
![](https://github.com/Evitha-CS/ElFraccionista-Videojuego/blob/main/Readme_images/App_ID.png)
- Una vez en ***Unity*** es que dirigirse a Window - > Photon Unity Networking - > PUN Wizard y pegar la ***APP ID*** antes copiada

![](https://github.com/Evitha-CS/ElFraccionista-Videojuego/blob/main/Readme_images/PUN_Wizard-Setup.png)

Finalmente se debe presionar ***Setup Project*** y ya estará todo configurado para utilizar el videojuego. :smile:

**NOTA:** Al estar utilizando ***Photon Unity Networking*** en una versión de prueba solo es posible que se conecten 20 usuarios simultaneamente, en el caso de que quieran ingresar más usuarios es que usar la versión de paga, o bien usar otro método para que el videojuego sea multijugador, lo cual no es recomendable, ya que requerirá editar todas las clases que utilicen librerias de Photon.

## Pasos a seguir antes de alojar en servidor 

- En el caso de que se quiera alojar en un servidor web, es necesario que se cambien algunas lineas de código para que el videojuego pueda enviar y recibir información correctamente desde la aplicación web:

  #### Paso 1:
  - Se deben cambiar las rutas en donde se guardan los datos de las partidas en las clases: ControlStartGame, VictoryManager y RoomUtilities:
    
  ![](https://github.com/Evitha-CS/ElFraccionista-Videojuego/blob/main/Readme_images/Cambiar_Rutas.png)
  ![](https://github.com/Evitha-CS/ElFraccionista-Videojuego/blob/main/Readme_images/Cambiar_Rutas2.png)

  #### Paso 2:
  - Hacer la build a WEBGL, asegurandose de que todas las escenas estén seleccionadas
    
  #### Paso 3:
  - Una vez hecha la build, se debe reemplazar la carpeta ***build*** anterior por la actual que se encuentra guardada dentro de la carpeta ***public*** en el proyecto de la aplicación web.
