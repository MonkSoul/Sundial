// Copyright (c) 2020-2021 ��Сɮ, Baiqian Co.,Ltd.
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
    /// Sundial ģ�������չ
    /// </summary>
    public static class SundialServiceCollectionExtensions
    {
        /// <summary>
        /// ��� Sundial ģ��ע��
        /// </summary>
        /// <param name="services">���񼯺϶���</param>
        /// <param name="configureOptionsBuilder">������ҵ����ѡ�����ί��</param>
        /// <returns>���񼯺�ʵ��</returns>
        public static IServiceCollection AddSundial(this IServiceCollection services, Action<SundialOptionsBuilder> configureOptionsBuilder)
        {
            // ������ʼ������ҵ����ѡ�����
            var sundialOptionsBuilder = new SundialOptionsBuilder();
            configureOptionsBuilder.Invoke(sundialOptionsBuilder);

            return services.AddSundial(sundialOptionsBuilder);
        }

        /// <summary>
        /// ��� Sundial ģ��ע��
        /// </summary>
        /// <param name="services">���񼯺϶���</param>
        /// <param name="sundialOptionsBuilder">������ҵ����ѡ�����</param>
        /// <returns>���񼯺�ʵ��</returns>
        public static IServiceCollection AddSundial(this IServiceCollection services, SundialOptionsBuilder sundialOptionsBuilder = default)
        {
            // ��ʼ��������ҵ������
            sundialOptionsBuilder ??= new SundialOptionsBuilder();

            // ע���ڲ�����
            services.AddInternalService();

            // ����������ҵ����
            sundialOptionsBuilder.Build(services);

            return services;
        }

        /// <summary>
        /// ע���ڲ�����
        /// </summary>
        /// <param name="services">���񼯺϶���</param>
        /// <returns>���񼯺�ʵ��</returns>
        private static IServiceCollection AddInternalService(this IServiceCollection services)
        {
            // ע����ҵ�洢�������ù�����ʽ����
            services.AddSingleton<IJobStorer>(_ =>
            {
                // ����Ĭ�ϻ����ڴ����ҵ�洢��
                return new MemoryJobStorer();
            });

            // ע����ҵ������
            services.AddSingleton<IScheduler, Scheduler>();

            return services;
        }
    }
}