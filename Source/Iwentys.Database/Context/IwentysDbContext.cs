using System.Collections.Generic;
using System.Linq;
using Iwentys.Models.Entities;
using Iwentys.Models.Entities.Guilds;
using Iwentys.Models.Entities.Study;
using Iwentys.Models.Types;
using Microsoft.EntityFrameworkCore;

namespace Iwentys.Database.Context
{
    public class IwentysDbContext : DbContext
    {
        public IwentysDbContext(DbContextOptions options) : base(options)
        {
        }

        #region Guilds

        public DbSet<Guild> Guilds { get; set; }
        public DbSet<GuildMember> GuildMembers { get; set; }
        public DbSet<GuildPinnedProject> GuildPinnedProjects { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Tribute> Tributes { get; set; }

        #endregion

        #region Study

        public DbSet<StudyGroup> StudyGroups { get; set; }
        public DbSet<StudyProgram> StudyPrograms { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectActivity> SubjectActivities { get; set; }
        public DbSet<SubjectForGroup> SubjectForGroups { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        #endregion

        public DbSet<Student> Students { get; set; }
        public DbSet<StudentProject> StudentProjects { get; set; }
        public DbSet<BarsPointTransactionLog> BarsPointTransactionLogs { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyWorker> CompanyWorkers { get; set; }

        public DbSet<Quest> Quests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetCompositeKeys(modelBuilder);

            modelBuilder.Entity<Guild>().HasIndex(g => g.Title).IsUnique();

            modelBuilder.Entity<GuildMember>().HasIndex(g => g.MemberId).IsUnique();
            modelBuilder.Entity<CompanyWorker>().HasIndex(g => g.WorkerId).IsUnique();

            modelBuilder.Entity<StudyProgram>().HasData(getStudyProgramsList());
            modelBuilder.Entity<StudyGroup>().HasData(getStudyGroupsList());
            modelBuilder.Entity<Teacher>().HasData(getTeachersList());
            modelBuilder.Entity<Subject>().HasData(getSubjectsList());
            modelBuilder.Entity<SubjectForGroup>().HasData(getSubjectForGroupsList());

            //TODO: fix
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }

        private void SetCompositeKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GuildMember>().HasKey(g => new {g.GuildId, g.MemberId});
            modelBuilder.Entity<CompanyWorker>().HasKey(g => new {g.CompanyId, g.WorkerId});
            modelBuilder.Entity<SubjectActivity>().HasKey(s => new {s.SubjectForGroupId, s.StudentId});
        }

        /// <summary>
        /// ��������� 5 ������� - ������ ���������� ��� �������� ���� ��� ����,
        /// ����� ������ � ��� ������ � �������, ������������ � �.�.
        /// ��� ������� �� ������� �� ��, ��� ����� ���������� ����� ����� �������� �
        /// �� ����� �� �� ����� �������� ����� API.
        /// TODO: ����� ������� ������ ���� ��� ������� ��������� ������ ������ � �������� ������ ������, � �� ��������� ����� � ����
        /// 
        /// </summary>
        /// <returns>������ ��������, ������� ����� �������� � ���� ��� ��������</returns>
        private List<StudyProgram> getStudyProgramsList()
        {
            var result = new List<StudyProgram> {new StudyProgram {Name = "��"}};

            return result;
        }
        private List<StudyGroup> getStudyGroupsList()
        {
            var result = new List<StudyGroup>
            {
                new StudyGroup
                {
                    Id = 1,
                    NamePattern = "�3201", StudyProgram = new StudyProgram {Id = 1, Name = "��"}, Year = 2020
                },
                new StudyGroup
                {
                    Id = 2,
                    NamePattern = "�3202", StudyProgram = new StudyProgram {Id = 1, Name = "��"}, Year = 2020
                },
                new StudyGroup
                {
                    Id = 3,
                    NamePattern = "�3203", StudyProgram = new StudyProgram {Id = 1, Name = "��"}, Year = 2020
                }
            };

            return result;
        }

        private List<Subject> getSubjectsList()
        {
            var result = new List<Subject>
            {
                new Subject {Name = "Programming"}, new Subject {Name = "Physical Culture"}
            };

            return result;
        }

        private List<Teacher> getTeachersList()
        {
            var result = new List<Teacher>
            {
                new Teacher {Name = "�������� ������� �����������"},
                new Teacher {Name = "������� ����� ������������"}
            };

            return result;
        }

        private List<SubjectForGroup> getSubjectForGroupsList()
        {
            var result = new List<SubjectForGroup>
            {
                new SubjectForGroup
                {
                    Subject = new Subject {Name = "Programming"},
                    StudyGroup =
                        new StudyGroup
                        {
                            NamePattern = "�3201",
                            StudyProgram = new StudyProgram {Name = "��"},
                            Year = 2020
                        },
                    Lecturer = new Teacher {Name = "������� ����� ������������"},
                    StudySemester = StudySemester.Y20H1
                },
                new SubjectForGroup
                {
                    Subject = new Subject {Name = "Physical Culture"},
                    StudyGroup =
                        new StudyGroup
                        {
                            NamePattern = "�3201",
                            StudyProgram = new StudyProgram {Name = "��"},
                            Year = 2020
                        },
                    Lecturer = new Teacher {Name = "�������� ������� �����������"},
                    StudySemester = StudySemester.Y20H1
                },
                new SubjectForGroup
                {
                    Subject = new Subject {Name = "Programming"},
                    StudyGroup =
                        new StudyGroup
                        {
                            NamePattern = "�3202",
                            StudyProgram = new StudyProgram {Name = "��"},
                            Year = 2020
                        },
                    Lecturer = new Teacher {Name = "������� ����� ������������"},
                    StudySemester = StudySemester.Y20H1
                },
                new SubjectForGroup
                {
                    Subject = new Subject {Name = "Physical Culture"},
                    StudyGroup =
                        new StudyGroup
                        {
                            NamePattern = "�3202",
                            StudyProgram = new StudyProgram {Name = "��"},
                            Year = 2020
                        },
                    Lecturer = new Teacher {Name = "�������� ������� �����������"},
                    StudySemester = StudySemester.Y20H1
                },
                new SubjectForGroup
                {
                    Subject = new Subject {Name = "Programming"},
                    StudyGroup =
                        new StudyGroup
                        {
                            NamePattern = "�3203",
                            StudyProgram = new StudyProgram {Name = "��"},
                            Year = 2020
                        },
                    Lecturer = new Teacher {Name = "������� ����� ������������"},
                    StudySemester = StudySemester.Y20H1
                },
                new SubjectForGroup
                {
                    Subject = new Subject {Name = "Physical Culture"},
                    StudyGroup =
                        new StudyGroup
                        {
                            NamePattern = "�3203",
                            StudyProgram = new StudyProgram {Name = "��"},
                            Year = 2020
                        },
                    Lecturer = new Teacher {Name = "�������� ������� �����������"},
                    StudySemester = StudySemester.Y20H1
                }
            };

            return result;
        }
    }
}