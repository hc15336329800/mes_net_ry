using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuoYi.Data.Models
{
    /// <summary>
    /// 通用的泛型树节点类，这样它就不再局限于 SysDept 类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeEl<T>
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public List<TreeEl<T>>? Children { get; set; }

        public TreeEl(T entity,Func<T,long> idSelector,Func<T,string> labelSelector,Func<T,List<T>> childrenSelector)
        {
            Id = idSelector(entity);
            Label = labelSelector(entity);
            var childs = childrenSelector(entity);
            if(childs != null && childs.Any())
            {
                Children = childs.Select(c => new TreeEl<T>(c,idSelector,labelSelector,childrenSelector)).ToList();
            }
        }

        // 用于序列化时忽略空 Children
        public bool ShouldSerializeChildren( ) => Children != null && Children.Any();
    }




}
