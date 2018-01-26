using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using StructureMap;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Users;

namespace Sunnet.Cli.Core.Users
{
    internal partial class UserService
    {
        #region Get Method
        public IQueryable<AffiliationEntity> Affiliations
        {
            get { return AffiliationRpst.Entities; }
        }

        public IQueryable<CertificateEntity> Certificates
        {
            get { return CertificateRpst.Entities; }
        }

        public IQueryable<PositionEntity> Positions
        {
            get { return PositionRpst.Entities; }
        }

        public IQueryable<YearsInProjectEntity> YearsInProjects
        {
            get { return YearsInProjectRpst.Entities; }
        }

        public IQueryable<ProfessionalDevelopmentEntity> ProfessionalDevelopments
        {
            get { return ProfessionalDevelopmentRpst.Entities; }
        }

        public ProfessionalDevelopmentEntity GetProfessionalDevelopment(int id)
        {
            return ProfessionalDevelopmentRpst.GetByKey(id);
        }

        public CertificateEntity GetCertificate(int id)
        {
            return CertificateRpst.GetByKey(id);
        }
        #endregion

        #region Delete Methos
        public OperationResult DeletePDs(List<ProfessionalDevelopmentEntity> listPD)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                ProfessionalDevelopmentRpst.Delete(listPD);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result; 
        }

        public OperationResult DeleteCertificates(List<CertificateEntity> listCertificate)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                CertificateRpst.Delete(listCertificate);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion


        /// <summary>
        /// 专供批量发送邮件程序使用
        /// </summary>
        /// <param name="sentTime">最多发送次数</param>
        public void ResetEmail( int sentTime)
        {
            UserRepository.ResetEmail(sentTime);
        }

        /// <summary>
        /// 专供批量发送邮件程序使用
        /// </summary>
        /// <param name="expireTimeDay"></param>
        public void UpdateInvitationEmail(int expirationDay, int userId)
        {
            UserRepository.UpdateInvitationEmail(expirationDay, userId);
        }


        /// <summary>
        /// 记录发送的Email记录
        /// </summary>
        /// <param name="log"></param>
        public void InsertEmailLog(EmailLogEntity log)
        {
            UserRepository.InsertEmailLog(log);
        }

        #region Position
        public OperationResult UpdatePosition(PositionEntity entity)
        {
            OperationResult result=new OperationResult(OperationResultType.Success);
            try
            {
                PositionRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertPosition(PositionEntity entity)
        {
            OperationResult result=new OperationResult(OperationResultType.Success);
            try
            {
                PositionRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region YearsInProject
        public OperationResult InsertYearsInProject(YearsInProjectEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                YearsInProjectRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateYearsInProject(YearsInProjectEntity entity)
        {
            OperationResult result=new OperationResult(OperationResultType.Success);
            try
            {
                YearsInProjectRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Certificate
        public OperationResult InsertCertificate(CertificateEntity entity, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                CertificateRpst.Insert(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCertificate(CertificateEntity entity, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                CertificateRpst.Update(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Affiliation
        public OperationResult InsertAffiliation(AffiliationEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AffiliationRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateAffiliation(AffiliationEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AffiliationRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region ProfessionalDevelopment
        public OperationResult InsertProfessionalDevelopment(ProfessionalDevelopmentEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                ProfessionalDevelopmentRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateProfessionalDevelopment(ProfessionalDevelopmentEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                ProfessionalDevelopmentRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion
    }
}
