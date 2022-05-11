<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li> 
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

![image](https://user-images.githubusercontent.com/21089760/167869303-e94b20dd-b6cd-45f1-bcd4-a3f09d10643e.png)

Şirketimizde bulunan legacy projelerin DAL (Data Access Layer) katmanında yapılan bazı kod tekrarlarını düzeltmek amacı ile yazılmış programdır.
Dizi olarak tanımlanan SqlParameter sınıfını Liste biçimine dönüştürerek tüm parameterelerini Collection Initializer Syntax'ına uygun şekilde setlemekte.
Böylece daha az kod satırı ile aynı işi yapmış oluyoruz ve SonarQube üzerinde çıkan Duplicated Blocks uyarılarını azaltmış oluyoruz. 
Bu kod iyileştirmelerini elle yapmak da mümkün fakat proje geneline bakılınca manuel düzeltmek çok efor alacağı için bu şekilde bir tool kullanmak daha mantıklı.

* Programa vereceğiniz path içerisindeki tüm "*.cs" uzantılı dosyalar taranır
* Taranan tüm doslar içinde değişiklik yapmaya uygun methodlar sırası ile değiştirilir.
* Direction = ParameterDirection.Input zaten default değer olduğu için yeni tanımlamalarda bu ifade çıkartılarak kod kısaltılır
* // ile başlayan tekli yorum satırları için işlem yapılmaz.
* Eski kod bloğu yenisi ile değiştirilir
* using System.Collections.Generic; namespace'i düzenleme yapılan sınıfta yoksa otomatik olarak sayfanın başına eklenir
* ![image](https://user-images.githubusercontent.com/21089760/167873895-5e9c015f-3591-400e-bf10-035ad0bb0c01.png)
* Return Value değerlerinin okunurken verilen statik index numarası dinamik hale getirilir
* ![image](https://user-images.githubusercontent.com/21089760/167874807-83f67afa-6689-400b-bff8-df4abb5a70ba.png)

Not: Değiştirilen her blok review edilmelidir, yanlış düzenleme yapılan bazı case'ler mevcuttur, tüm case'ler handle edilmediği için herhangi bir yeri bozmamak için iyi review yapılması gerekmetedir.


 <p align="right">(<a href="#top">back to top</a>)</p>

### Built With

This section should list any major frameworks/libraries used to bootstrap your project. Leave any add-ons/plugins for the acknowledgements section. Here are a few examples.

* [C#](https://docs.microsoft.com/tr-tr/dotnet/csharp/)
* [.net core 5](https://docs.microsoft.com/tr-tr/dotnet/core/whats-new/dotnet-5) 
 
 <p align="right">(<a href="#top">back to top</a>)</p>
<!-- GETTING STARTED -->
## Getting Started

Uygulamayı lokalinizi indirerek direkt çalıştırabilirsiniz

### Prerequisites

This is an example of how to list things you need to use the software and how to install them.
* Visual Studio

### Installation
 
1. Clone the repo
   ```sh
   git clone https://github.com/adildeveci/DalRefactor.git
   ```
   
<p align="right">(<a href="#top">back to top</a>)</p>

<!-- USAGE EXAMPLES -->
## Usage

* Application Arguments bölümüne dönüştürmek istediğiniz path'i yazın
* ![image](https://user-images.githubusercontent.com/21089760/167872443-d5e0bab2-876e-4273-b127-d457889e2b96.png)
* Programı çalıştırın
 
 <p align="right">(<a href="#top">back to top</a>)</p>

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

Adil Deveci - [LinkedIn](https://www.linkedin.com/in/adildeveci/) - adildeveci@hotmail.com

Project Link: [https://github.com/adildeveci/DalRefactor](https://github.com/adildeveci/DalRefactor)

<p align="right">(<a href="#top">back to top</a>)</p>
