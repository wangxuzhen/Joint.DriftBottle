using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Mvc;
using Joint.Entity;
using Joint.Web.Framework;
using Joint.Common;
using System.Web;
using Joint.IService;
using Joint.DLLFactory;
using System.Collections.Generic;
using System.Web.Mvc.Html;
using System.Linq;

namespace Joint.Web
{
    public static class MVCHelpers
    {

        /// <summary>
        /// 分页控件
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pagingInfo"></param>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        public static MvcHtmlString PageLinks2(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
        {
            //如果没有数据，则不显示分页
            if (pagingInfo.Total == 0)
            {
                return MvcHtmlString.Create("");
            }

            StringBuilder builder = new StringBuilder();
            if (pagingInfo.TotalPages != 0)
            {
                TagBuilder builder5;
                int num2;
                int num3;
                TagBuilder builder6;
                TagBuilder builder2 = new TagBuilder("a")
                {
                    InnerHtml = "上一页"
                };
                if (pagingInfo.CurrentPage != 1)
                {
                    builder2.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage - 1));
                }
                else
                {
                    builder2.MergeAttribute("class", "prev-disabled");
                }
                builder.Append(builder2.ToString());
                if (pagingInfo.TotalPages >= 1)
                {
                    TagBuilder builder3 = new TagBuilder("a")
                    {
                        InnerHtml = "1"
                    };
                    builder3.MergeAttribute("href", pageUrl(1));
                    if (pagingInfo.CurrentPage == 1)
                    {
                        builder3.MergeAttribute("class", "current");
                    }
                    builder.Append(builder3.ToString());
                }
                if (pagingInfo.TotalPages >= 2)
                {
                    TagBuilder builder4 = new TagBuilder("a")
                    {
                        InnerHtml = "2"
                    };
                    builder4.MergeAttribute("href", pageUrl(2));
                    if (pagingInfo.CurrentPage == 2)
                    {
                        builder4.MergeAttribute("class", "current");
                    }
                    builder.Append(builder4.ToString());
                }
                if ((pagingInfo.CurrentPage > 5) && (pagingInfo.TotalPages != 6))
                {
                    builder5 = new TagBuilder("span")
                    {
                        InnerHtml = "..."
                    };
                    builder5.MergeAttribute("class", "text");
                    builder.Append(builder5.ToString());
                }
                int num = (pagingInfo.CurrentPage > 2) ? pagingInfo.CurrentPage : 3;
                if (pagingInfo.CurrentPage <= 5)
                {
                    num2 = 3;
                    for (num3 = num2; (num3 < 8) && (num3 <= pagingInfo.TotalPages); num3++)
                    {
                        builder6 = new TagBuilder("a")
                        {
                            InnerHtml = num3.ToString()
                        };
                        builder6.MergeAttribute("href", pageUrl(num3));
                        if (num3 == pagingInfo.CurrentPage)
                        {
                            builder6.MergeAttribute("class", "current");
                        }
                        builder.Append(builder6.ToString());
                    }
                    if (pagingInfo.TotalPages > 7)
                    {
                        builder5 = new TagBuilder("span")
                        {
                            InnerHtml = "..."
                        };
                        builder5.MergeAttribute("class", "text");
                        builder.Append(builder5.ToString());
                    }
                }
                if ((pagingInfo.CurrentPage > 5) && ((pagingInfo.CurrentPage + 5) > pagingInfo.TotalPages))
                {
                    num2 = pagingInfo.TotalPages - 4;
                    if (num2 == 2)
                    {
                        num2++;
                    }
                    for (num3 = num2; num3 <= pagingInfo.TotalPages; num3++)
                    {
                        builder6 = new TagBuilder("a")
                        {
                            InnerHtml = num3.ToString()
                        };
                        builder6.MergeAttribute("href", pageUrl(num3));
                        if (num3 == pagingInfo.CurrentPage)
                        {
                            builder6.MergeAttribute("class", "current");
                        }
                        builder.Append(builder6.ToString());
                    }
                }
                if ((pagingInfo.CurrentPage > 5) && ((pagingInfo.CurrentPage + 5) <= pagingInfo.TotalPages))
                {
                    for (num3 = pagingInfo.CurrentPage; num3 < (pagingInfo.CurrentPage + 5); num3++)
                    {
                        builder6 = new TagBuilder("a");
                        builder6.InnerHtml = (num3 - 2).ToString();
                        builder6.MergeAttribute("href", pageUrl(num3 - 2));
                        if (num3 == (pagingInfo.CurrentPage + 2))
                        {
                            builder6.MergeAttribute("class", "current");
                        }
                        builder.Append(builder6.ToString());
                    }
                    builder5 = new TagBuilder("span")
                    {
                        InnerHtml = "..."
                    };
                    builder5.MergeAttribute("class", "text");
                    builder.Append(builder5.ToString());
                }
                TagBuilder builder7 = new TagBuilder("a")
                {
                    InnerHtml = "下一页"
                };
                if (pagingInfo.CurrentPage != pagingInfo.TotalPages)
                {
                    builder7.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage + 1));
                }
                else
                {
                    builder7.MergeAttribute("class", "next-disabled");
                }
                builder.Append(builder7.ToString());
            }
            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// 复杂分页，带查询条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html"></param>
        /// <param name="pagingInfo"></param>
        /// <param name="pageUrl"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl, object searchModel)
        {
            //如果没有数据，则不显示分页
            if (pagingInfo.Total == 0)
            {
                return MvcHtmlString.Create("");
            }

            string urlParams = string.Empty;
            if (searchModel != null)
            {
                urlParams = UrlParamsHelper.GetProperties(searchModel);
            }

            UrlParamsHelper.GetProperties(searchModel);
            StringBuilder sbPage = new StringBuilder();
            sbPage.Append("<ul class=\"pagination\">");
            if (pagingInfo.CurrentPage != 1)
            {
                sbPage.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", pageUrl(1) + urlParams, "«");
            }
            else
            {
                string isDisabled = "class=\"disabled\"";
                sbPage.AppendFormat("<li {0}><span>{1}</span></li>", isDisabled, "«");
            }

            //总共显示10条分页1-10页
            int showPage = 10;
            if (pagingInfo.TotalPages <= showPage)//小于10页的时候
            {
                for (int i = 1; i <= pagingInfo.TotalPages; i++)
                {
                    if (pagingInfo.CurrentPage == i)
                    {
                        string isActiveClass = "class=\"active\"";
                        sbPage.AppendFormat("<li {0}><span>{1}</span></li>", isActiveClass, i);
                    }
                    else
                    {
                        sbPage.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", pageUrl(i) + urlParams, i);
                    }
                }
            }
            else//如果总分页大于10页
            {
                //如果分页在前五页
                if (pagingInfo.CurrentPage <= (int)Math.Ceiling(showPage / 2.0))
                {
                    for (int i = 1; i <= showPage; i++)
                    {
                        if (pagingInfo.CurrentPage == i)
                        {
                            string isActiveClass = "class=\"active\"";
                            sbPage.AppendFormat("<li {0}><span>{1}</span></li>", isActiveClass, i);
                        }
                        else
                        {
                            sbPage.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", pageUrl(i) + urlParams, i);
                        }
                    }
                }
                else//如果当前分页大于第五页的时候，就要判断最后的一个分页是否超出总分页
                {
                    if (pagingInfo.CurrentPage + showPage / 2 < pagingInfo.TotalPages)
                    {
                        for (int i = pagingInfo.CurrentPage - (int)Math.Ceiling(showPage / 2.0); i < pagingInfo.CurrentPage + showPage / 2; i++)
                        {
                            if (pagingInfo.CurrentPage == i)
                            {
                                string isActiveClass = "class=\"active\"";
                                sbPage.AppendFormat("<li {0}><span>{1}</span></li>", isActiveClass, i);
                            }
                            else
                            {
                                sbPage.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", pageUrl(i) + urlParams, i);
                            }
                        }
                    }
                    else//如果当前加5页已经超出总页码
                    {
                        for (int i = pagingInfo.TotalPages - showPage + 1; i <= pagingInfo.TotalPages; i++)
                        {
                            if (pagingInfo.CurrentPage == i)
                            {
                                string isActiveClass = "class=\"active\"";
                                sbPage.AppendFormat("<li {0}><span>{1}</span></li>", isActiveClass, i);
                            }
                            else
                            {
                                sbPage.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", pageUrl(i) + urlParams, i);
                            }
                        }
                    }
                }
            }
            if (pagingInfo.CurrentPage != pagingInfo.TotalPages)
            {

                sbPage.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", pageUrl(pagingInfo.TotalPages) + urlParams, "»");
            }
            else
            {
                string isDisabled2 = "class=\"disabled\"";
                sbPage.AppendFormat("<li {0}><span>{1}</span></li>", isDisabled2, "»");
            }

            sbPage.Append("</ul>");
            return MvcHtmlString.Create(sbPage.ToString());
        }


        /// <summary>
        /// 分页控件
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pagingInfo"></param>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
        {
            return PageLinks(html, pagingInfo, pageUrl, null);
            #region 原始分页代码注释备用
            //StringBuilder sbPage = new StringBuilder();
            //sbPage.Append("<ul class=\"pagination\">");
            //if (pagingInfo.CurrentPage != 1)
            //{
            //    sbPage.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", pageUrl(1), "«");
            //}
            //else
            //{
            //    string isDisabled = "class=\"disabled\"";
            //    sbPage.AppendFormat("<li {0}><span>{1}</span></li>", isDisabled, "«");
            //}

            ////总共显示10条分页1-10页
            //int showPage = 10;
            ////小于10页的时候
            //if (pagingInfo.TotalPages <= showPage)
            //{
            //    for (int i = 1; i <= pagingInfo.TotalPages; i++)
            //    {
            //        if (pagingInfo.CurrentPage == i)
            //        {
            //            string isActiveClass = "class=\"active\"";
            //            sbPage.AppendFormat("<li {0}><span>{1}</span></li>", isActiveClass, i);
            //        }
            //        else
            //        {
            //            sbPage.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", pageUrl(i), i);
            //        }
            //    }
            //}
            //else
            //{
            //    if (pagingInfo.CurrentPage <= (int)Math.Ceiling(showPage / 2.0))
            //    {
            //        for (int i = 1; i <= showPage; i++)
            //        {
            //            if (pagingInfo.CurrentPage == i)
            //            {
            //                string isActiveClass = "class=\"active\"";
            //                sbPage.AppendFormat("<li {0}><span>{1}</span></li>", isActiveClass, i);
            //            }
            //            else
            //            {
            //                sbPage.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", pageUrl(i), i);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        for (int i = pagingInfo.CurrentPage - (int)Math.Ceiling(showPage / 2.0); i < pagingInfo.CurrentPage + showPage / 2; i++)
            //        {
            //            if (pagingInfo.CurrentPage == i)
            //            {
            //                string isActiveClass = "class=\"active\"";
            //                sbPage.AppendFormat("<li {0}><span>{1}</span></li>", isActiveClass, i);
            //            }
            //            else
            //            {
            //                sbPage.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", pageUrl(i), i);
            //            }
            //        }
            //    }
            //}
            //if (pagingInfo.CurrentPage != pagingInfo.TotalPages)
            //{

            //    sbPage.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", pageUrl(pagingInfo.TotalPages), "»");
            //}
            //else
            //{
            //    string isDisabled2 = "class=\"disabled\"";
            //    sbPage.AppendFormat("<li {0}><span>{1}</span></li>", isDisabled2, "»");
            //}

            //sbPage.Append("</ul>");
            //return MvcHtmlString.Create(sbPage.ToString()); 
            #endregion
        }

        public static MvcHtmlString PageShow(this HtmlHelper html, PagingInfo pagingInfo)
        {
            StringBuilder builder = new StringBuilder();
            if (pagingInfo.TotalPages != 0)
            {
                builder.Append(string.Concat(new object[] { "<font color='#ff6600'><b>", pagingInfo.CurrentPage, "</b></font> / ", pagingInfo.TotalPages, " 页，每页<font color='#ff6600'><b>", pagingInfo.PageSize, "</b></font> 条，共 <font color='#ff6600'><b>", pagingInfo.Total, "</b></font> 条" }));
            }
            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// DropDownList添加一个扩展方法，支持SelectListItemModel转换
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="list">SelectListItemModel列表</param>
        /// <param name="htmlAttribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItemModel> list, object htmlAttribute)
        {
            IList<SelectListItem> selectItem = new List<SelectListItem>();

            selectItem = list.Select(t=>new SelectListItem() {
                Text=t.Text,
                Value=t.Value
                
            }).ToList();

            return htmlHelper.DropDownList(name, selectItem, htmlAttribute);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItemModel> list,string optionLabel, object htmlAttribute)
        {
            IList<SelectListItem> selectItem = new List<SelectListItem>();

            selectItem = list.Select(t => new SelectListItem()
            {
                Text = t.Text,
                Value = t.Value

            }).ToList();

            return htmlHelper.DropDownList(name, selectItem,optionLabel, htmlAttribute);
        }
    }
}