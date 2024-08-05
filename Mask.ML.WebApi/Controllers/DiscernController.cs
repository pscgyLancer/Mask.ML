using Mask_MLML.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace Mask.ML.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscernController : ControllerBase
    {
        /// <summary>
        /// 上传文件：口罩验证
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file)
        {
            //申明返回的结果
            string result = "";

            //做随机数，用到文件夹名字上，防重名
            Random random = new Random();
            string r = "";
            int i;
            for (i = 1; i < 11; i++)
            {
                r += random.Next(0, 9).ToString();
            }
            //文件路径
            string FilePath = AppDomain.CurrentDomain.BaseDirectory + "/TempFiles/";
            string name = file.FileName;
            string FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + r;
            //获取文件类型
            string type = System.IO.Path.GetExtension(name);
            DirectoryInfo di = new DirectoryInfo(FilePath);
            if (!di.Exists)
            {
                di.Create();
            }
            //文件保存的路径
            var filefullname = FilePath + FileName + type;
            using (FileStream fs = System.IO.File.Create(filefullname))
            {
                // 复制文件
                file.CopyTo(fs);
                // 清空缓冲区数据
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
            //成功提示赋值到返回结果中
            //result = "文件上传成功";

            // 创建样例数据的单个实例对模型输入数据集的第一行
            ModelInput sampleData = new ModelInput()
            {
                ImageSource = filefullname,
            };
            // 获取预测结果
            var predictionResult = ConsumeModel.Predict(sampleData);
            //System.IO.File.Delete(filefullname);
            return Ok(predictionResult.Prediction);

        }

    }
}

