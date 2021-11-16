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
    /// SchedulerJob ģ�������չ
    /// </summary>
    public static class SchedulerJobServiceCollectionExtensions
    {
        /// <summary>
        /// ��� SchedulerJob ģ��ע��
        /// </summary>
        /// <param name="services">���񼯺϶���</param>
        /// <param name="configureOptionsBuilder">������ҵ����ѡ�����ί��</param>
        /// <returns>���񼯺�ʵ��</returns>
        public static IServiceCollection AddSchedulerJob(this IServiceCollection services, Action<SchedulerJobOptionsBuilder> configureOptionsBuilder)
        {
            // ������ʼ������ҵ����ѡ�����
            var schedulerJobOptionsBuilder = new SchedulerJobOptionsBuilder();
            configureOptionsBuilder.Invoke(schedulerJobOptionsBuilder);

            return services.AddSchedulerJob(schedulerJobOptionsBuilder);
        }

        /// <summary>
        /// ��� SchedulerJob ģ��ע��
        /// </summary>
        /// <param name="services">���񼯺϶���</param>
        /// <param name="schedulerJobOptionsBuilder">������ҵ����ѡ�����</param>
        /// <returns>���񼯺�ʵ��</returns>
        public static IServiceCollection AddSchedulerJob(this IServiceCollection services, SchedulerJobOptionsBuilder schedulerJobOptionsBuilder = default)
        {
            // ��ʼ��������ҵ������
            schedulerJobOptionsBuilder ??= new SchedulerJobOptionsBuilder();

            // ע���ڲ�����
            services.AddInternalService();

            // ����������ҵ����
            schedulerJobOptionsBuilder.Build(services);

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
            services.AddSingleton<ISchedulerJob, SchedulerJob>();

            return services;
        }
    }
}