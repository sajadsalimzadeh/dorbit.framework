﻿using System;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts;
using Dorbit.Framework.Contracts.Messages;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services.Abstractions;
using Kavenegar.Models;

namespace Dorbit.Framework.Services.MessageProviders;

[ServiceRegister]
public class KavenegarProvider : IMessageProvider<MessageSmsRequest>
{
    public string Name => "Kavenegar";
    private string _apKey;
    private string _sender;

    private class SendResponse
    {
        public string MessageId { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string Sender { get; set; }
        public long Date { get; set; }
        public int Cost { get; set; }
    }

    public void Configure(AppSettingMessageProvider configuration)
    {
        _sender = configuration.Sender;
        _apKey = configuration.ApiKey.GetDecryptedValue();
    }

    public async Task<QueryResult<string>> Send(MessageSmsRequest request)
    {
        SendResult result;
        if (string.IsNullOrEmpty(request.TemplateId))
        {
            var api = new Kavenegar.KavenegarApi(_apKey);
            result = api.Send(_sender, request.To, request.Text);
        }
        else
        {
            var api = new Kavenegar.KavenegarApi(_apKey);
            result = api.VerifyLookup(request.To, request.Args[0], request.TemplateId);
        }
        return result.Status switch
        {
            6 => throw new Exception("خطا در ارسال پیام که توسط سر شماره پیش می آید و به معنی عدم رسیدن پیامک می باشد"),
            11 => throw new Exception("نرسیده به گیرنده ، این وضعیت به دلایلی از جمله خاموش یا خارج از دسترس بودن گیرنده اتفاق می افتد "),
            13 => throw new Exception("ارسال پیام از سمت کاربر لغو شده یا در ارسال آن مشکلی پیش آمده که هزینه آن به حساب برگشت داده می شود"),
            14 => throw new Exception("بلاک شده است، عدم تمایل گیرنده به دریافت پیامک از خطوط تبلیغاتی که هزینه آن به حساب برگشت داده می شود"),
            100 => throw new Exception(
                "شناسه پیامک نامعتبر است ( به این معنی که شناسه پیام در پایگاه داده کاوه نگار ثبت نشده است یا متعلق به شما نمی باشد)"),
            _ => new QueryResult<string>(result.Messageid.ToString())
        };
    }
}