﻿using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using ZKWeb.Model;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Core {
	/// <summary>
	/// 翻译文本的帮助对象
	/// 实际翻译会在转换到字符串时执行，使用T类型保存文本可以用于延迟翻译
	/// 这个类可以直接转换到string，也可以使用ToString转换
	/// </summary>
	public struct T {
		/// <summary>
		/// 翻译前的文本
		/// </summary>
		private string Text { get; set; }

		/// <summary>
		/// 翻译文本
		/// </summary>
		/// <param name="text">文本</param>
		public T(string text) {
			Text = text;
		}

		/// <summary>
		/// 获取翻译后的文本
		/// </summary>
		/// <param name="t"></param>
		public static implicit operator string (T t) {
			return t.ToString();
		}

		/// <summary>
		/// 获取翻译后的文本
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			// 获取当前线程的语言
			var cluture = Thread.CurrentThread.CurrentCulture;
			// 获取翻译提供器并进行翻译
			// 支持 {语言}-{地区} 和 {语言} 代码
			foreach (var code in new[] { cluture.Name, cluture.TwoLetterISOLanguageName }) {
				var provides = Application.Ioc.ResolveMany<ITranslateProvider>(serviceKey: code);
				// 后注册的先翻译
				foreach (var provider in provides.Reverse()) {
					var translated = provider.Translate(Text);
					if (translated != null) {
						return translated;
					}
				}
			}
			// 没有找到翻译，返回原有的文本
			return Text;
		}
	}
}
