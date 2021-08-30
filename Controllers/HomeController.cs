using AuvoTech.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text;

namespace AuvoTech.Controllers
{
    public class HomeController : Controller
    {
        //Define uma instância de IHostingEnvironment
        IHostingEnvironment _appEnvironment;
        //Injeta a instância no construtor para poder usar os recursos
        public HomeController(IHostingEnvironment env)
        {
            _appEnvironment = env;
        }
        //Retorna a View Index.cshtml que será o formulário para
        //selecionar os arquivos a serem enviados 
        public IActionResult Index()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //método para enviar os arquivos usando a interface IFormFile
        public async Task<IActionResult> EnviarArquivo(List<IFormFile> arquivos)
        {

            // processa os arquivo enviados
            //percorre a lista de arquivos selecionados
            foreach (var arquivo in arquivos)
            {
                //verifica se existem arquivos 
                if (arquivo == null || arquivo.Length == 0)
                {
                    //retorna a viewdata com erro
                    ViewData["Erro"] = "Error: Arquivo(s) não selecionado(s)";
                    return View(ViewData);
                }
                //verifica qual o tipo de arquivo enviado
                if (!arquivo.FileName.Contains(".ofx"))
                {
                    ViewData["Error"] = "Aviso: Arquivo Não Suportado. Somente arquivos do tipo '.ofx' são aceitos na aplicação. Tente novamente.";
                    return View(ViewData);
                }

                string wwwRoot   = _appEnvironment.WebRootPath;
                string fileDb    = "\\Arquivos\\";
                string fileDest  = "\\Recebidos\\";
                string fileName  = arquivo.FileName.ToString();

                using (var stream = new FileStream(wwwRoot + fileDb + fileDest + fileName, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }
            }

            //monta a ViewData que será exibida na view como resultado do envio 
            ViewData["Resultado"] = $"{arquivos.Count} arquivos foram enviados ao servidor, " +
             $"com tamanho total de : x bytes";

            //retorna a viewdata
            return View(ViewData);
        }

    }
}
