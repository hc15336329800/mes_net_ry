using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuoYi.Data.Tool
{
    /// <summary>
    /// 名称：简化版的 ID 生成器(long类型并且防止前端截断精度)
    /// 说明：利用当前 Unix 时间戳（秒级）加上一个自增序号生成 13 位的 long 类型数字。
    /// </summary>
    public static class NextId
    {
        private static readonly object _lock = new object();
        private static long _lastTimestamp = 0;
        private static int _sequence = 0;

        /// <summary>
        /// 生成一个13位的唯一ID。格式为：当前Unix时间戳(秒) + 三位序号（000-999）
        /// 如果同一秒内生成超过1000个ID，则会等待下一秒。
        /// </summary>
        public static long Id13( )
        {
            lock(_lock)
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if(timestamp == _lastTimestamp)
                {
                    _sequence++;
                    if(_sequence >= 1000)
                    {
                        // 等待下一秒
                        while(timestamp == _lastTimestamp)
                        {
                            System.Threading.Thread.Sleep(1);
                            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        }
                        _sequence = 0;
                    }
                }
                else
                {
                    _sequence = 0;
                }
                _lastTimestamp = timestamp;
                // 生成ID：将秒级时间戳和3位序号拼接起来
                long id = long.Parse($"{timestamp}{_sequence:D3}");
                return id;
            }
        }
    }

}
