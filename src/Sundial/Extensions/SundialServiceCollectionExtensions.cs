// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Sundial is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Sundial;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Sundial 模块服务拓展
    /// </summary>
    public static class SundialServiceCollectionExtensions
    {
        /// <summary>
        /// 添加 Sundial 模块注册
        /// </summary>
        /// <param name="services">服务集合对象</param>
        /// <param name="configureOptionsBuilder">调度作业配置选项构建器委托</param>
        /// <returns>服务集合实例</returns>
        public static IServiceCollection AddSundial(this IServiceCollection services, Action<SundialOptionsBuilder> configureOptionsBuilder)
        {
            // 创建初始调度作业配置选项构建器
            var sundialOptionsBuilder = new SundialOptionsBuilder();
            configureOptionsBuilder.Invoke(sundialOptionsBuilder);

            return services.AddSundial(sundialOptionsBuilder);
        }

        /// <summary>
        /// 添加 Sundial 模块注册
        /// </summary>
        /// <param name="services">服务集合对象</param>
        /// <param name="sundialOptionsBuilder">调度作业配置选项构建器</param>
        /// <returns>服务集合实例</returns>
        public static IServiceCollection AddSundial(this IServiceCollection services, SundialOptionsBuilder sundialOptionsBuilder = default)
        {
            // 初始化调度作业配置项
            sundialOptionsBuilder ??= new SundialOptionsBuilder();

            // 注册内部服务
            services.AddInternalService();

            // 构建调度作业服务
            sundialOptionsBuilder.Build(services);

            return services;
        }

        /// <summary>
        /// 注册内部服务
        /// </summary>
        /// <param name="services">服务集合对象</param>
        /// <returns>服务集合实例</returns>
        private static IServiceCollection AddInternalService(this IServiceCollection services)
        {
            // 注册作业存储器，采用工厂方式创建
            services.AddSingleton<IJobStorer>(_ =>
            {
                // 创建默认基于内存的作业存储器
                return new MemoryJobStorer();
            });

            // 注册作业调度器
            services.AddSingleton<IScheduler, Scheduler>();

            return services;
        }
    }
}