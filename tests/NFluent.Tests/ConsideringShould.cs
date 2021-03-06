﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ConsideringRelatedTests.cs" company="NFluent">
//   Copyright 2018 Thomas PIERRAIN & Cyrille DUPUYDAUBY
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NFluent.Tests
{
    using Helpers;
    using NUnit.Framework;

    [TestFixture]
    public class ConsideringShould
    {
        private class SutClass
        {
            private static int autoInc;

            public int TheField;
            private int thePrivateField;

            public SutClass(int theField, int theProperty)
            {
                this.TheField = theField;
                this.TheProperty = theProperty;
                this.thePrivateField = autoInc++;
            }

            public SutClass(int theField, int theProperty, int thePrivateField, object thePrivateProperty)
            {
                this.TheField = theField;
                this.TheProperty = theProperty;
                this.ThePrivateProperty = thePrivateProperty;
                this.thePrivateField = thePrivateField;
            }

            protected internal object ThePrivateProperty { get; }

            public int TheProperty { get; }
        }

        [Test]
        public void NotShouldWorkWhenMissingMember()
        {
            var sut = new SutClass(2, 42);
            var expected = new {TheProperty = 12, Test = 11};

            Check.That(sut).Considering().Public.Properties.IsNoInstanceOfType(expected.GetType());
            Check.That(expected).Considering().Public.Properties.IsNoInstanceOfType(sut.GetType());
        }

        [Test]
        public void NotShouldFailWhenSame()
        {
            var sut = new SutClass(2, 42);
            var expected = new {TheProperty = 12};
            Check.ThatCode(() =>
            {
                Check.That(sut).Considering().Public.Properties.IsNoInstanceOfType(expected.GetType());
            }).FailsWithMessage(
                    "", "The checked value is an instance of [<>f__AnonymousType1<int>] whereas it must not.", "The checked value:", "\t[{ TheProperty = 42 }] of type: [NFluent.Helpers.ReflectionWrapper]", "The expected value: different from", "\tan instance of type: [<>f__AnonymousType1<int>]");
        }

        [Test]
        public void DetectDifferentProperties()
        {
            Check.ThatCode(() =>
                    Check.That(new SutClass(2, 43)).Considering().NonPublic.Properties
                        .IsEqualTo(new {ThePrivateProperty = (object) null}))
                .FailsWithMessage("", "The checked value's property 'ThePrivateProperty' is absent from the expected one.", "The checked property 'ThePrivateProperty':", "\t[null]");
            Check.ThatCode(() =>
                    Check.That(new {ThePrivateProperty = (object) null}).Considering().NonPublic.Properties
                        .IsEqualTo(new SutClass(2, 43)))
                .FailsWithMessage("", "The expected one's property 'ThePrivateProperty' is absent from the checked value.", "The expected property 'ThePrivateProperty':", "\t[null]");
        }

        [Test]
        public void DetectMissingProperties()
        {
            Check.ThatCode(() =>
                    Check.That(new SutClass(2, 43)).Considering().All.Properties
                        .IsEqualTo(new {ThePrivateProperty = (object) null}))
                .FailsWithMessage("", "The checked value's property 'TheProperty' is absent from the expected one.", "The checked property 'TheProperty':", "\t[43]");
            Check.ThatCode(() =>
                    Check.That(new {ThePrivateProperty = (object) null}).Considering().All.Properties
                        .IsEqualTo(new SutClass(2, 43)))
                .FailsWithMessage("", "The expected one's property 'TheProperty' is absent from the checked value.", "The expected property 'TheProperty':", "\t[43]");
        }

        [Test]
        public void FailForAllMembers()
        {
            var sut = new SutClass(2, 42, 4, null);
            Check.ThatCode(() =>
            {
                Check.That(sut).Considering().All.Fields.And.All.Properties
                    .IsEqualTo(new SutClass(2, 42, 4, new object()));
            }).FailsWithMessage("", "The checked value's autoproperty 'ThePrivateProperty' (field '<ThePrivateProperty>k__BackingField') does not have the expected value.", "The checked value:", "\t[null]", "The expected value:", "\t[System.Object] of type: [object]");
        }

        [Test]
        public void FailForDifferentPublicFields()
        {
            var sut = new SutClass(2, 42);

            Check.ThatCode(() =>
                    Check.That(sut).Considering().Public.Fields.IsEqualTo(new SutClass(3, 42)))
                .FailsWithMessage("", "The checked value's field 'TheField' does not have the expected value.", "The checked value:", "\t[2]", "The expected value:", "\t[3]");
        }

        [Test]
        public void FailForDifferentPublicProperties()
        {
            var sut = new SutClass(2, 42);

            Check.ThatCode(() =>
                    Check.That(sut).Considering().Public.Properties.IsEqualTo(new SutClass(2, 43)))
                .FailsWithMessage("", "The checked value's property 'TheProperty' does not have the expected value.", "The checked value:", "\t[42]", "The expected value:", "\t[43]");
        }

        [Test]
        public void FailOnNullForAllMembers()
        {
            var sut = new SutClass(2, 42, 4, null);

            var expected = new SutClass(2, 42, 4, sut);
            Check.ThatCode(() => Check.That(sut).Considering().All.Fields.And.All.Properties.IsEqualTo(expected))
                .FailsWithMessage("", "The checked value's autoproperty 'ThePrivateProperty' (field '<ThePrivateProperty>k__BackingField') does not have the expected value.", "The checked value:", "\t[null]", "The expected value:", "\t[NFluent.Tests.ConsideringShould+SutClass] of type: [NFluent.Tests.ConsideringShould+SutClass]");
            Check.ThatCode(() => Check.That(expected).Considering().All.Fields.And.All.Properties.IsEqualTo(sut))
                .FailsWithMessage("", "The checked value's autoproperty 'ThePrivateProperty' (field '<ThePrivateProperty>k__BackingField') does not have the expected value.", "The checked value:", "\t[NFluent.Tests.ConsideringShould+SutClass]", "The expected value:", "\t[null]");
        }

        [Test]
        public void FailWhenMissingMember()
        {
            var sut = new SutClass(2, 42);
            var expected = new {TheProperty = 12, Test = 11};
            Check.ThatCode(() =>
            {
                Check.That(sut).Considering().Public.Properties.IsInstanceOfType(expected.GetType());
            }).FailsWithMessage("", "The expected one's property 'Test' is absent from the checked value.", "The expected property 'Test':", "\t[null]");
            Check.ThatCode(() =>
            {
                Check.That(expected).Considering().Public.Properties.IsInstanceOfType(sut.GetType());
            }).FailsWithMessage("", "The checked value's property 'Test' is absent from the expected one.", "The checked property 'Test':", "\t[11]");
        }

        [Test]
        public void WorkForAllMembers()
        {
            var sut = new SutClass(2, 42, 4, null);

            Check.That(sut).Considering().All.Fields.And.All.Properties.IsEqualTo(new SutClass(2, 42, 4, null));
        }

        [Test]
        public void WorkWhenDoubled()
        {
            var sut = new SutClass(2, 42, 4, null);

            Check.That(sut).Considering().All.Fields.And.All.Properties.Considering().All.Fields.And.All.Properties.IsEqualTo(new SutClass(2, 42, 4, null));
        }

        [Test]
        public void WorkForIdenticalPublicFields()
        {
            var sut = new SutClass(2, 42);

            Check.That(sut).Considering().Public.Fields.IsEqualTo(new SutClass(2, 42));
        }

        [Test]
        public void ShouldWorkForIdenticalPublicFieldsAndDifferentProperties()
        {
            var sut = new SutClass(2, 42);

            Check.That(sut).Considering().Public.Fields.IsEqualTo(new SutClass(2, 43));
        }


        [Test]
        public void WorkForIdenticalPublicProperties()
        {
            var sut = new SutClass(2, 42);

            Check.That(sut).Considering().Public.Properties.IsEqualTo(new SutClass(1, 42));
        }

        [Test]
        public void WorkForIsInstanceOf()
        {
            var sut = new SutClass(2, 42);
            var expected = new {TheProperty = 12};
            Check.That(expected).Considering().Public.Properties.IsInstanceOf<SutClass>();
        }

        [Test]
        public void ShouldWorkForIsInstanceOfType()
        {
            var sut = new SutClass(2, 42);
            var expected = new {TheProperty = 12};
            Check.That(sut).Considering().Public.Properties.IsInstanceOfType(expected.GetType());
            Check.That(expected).Considering().Public.Properties.IsInstanceOfType(sut.GetType());
        }

        [Test]
        public void WorkForNull()
        {
            var sut = new SutClass(2, 42);
            Check.That(sut).Considering().Public.Fields.Equals(null);
        }

        [Test]
        public void WorkForOtherChecks()
        {
            var sut = new SutClass(2, 42);
            Check.That(sut).Considering().Public.Fields.Equals(new SutClass(2, 42));
            Check.That(sut).Considering().All.Fields.Not.Equals(new SutClass(2, 42));
        }

        [Test]
        public void WorkForPrivateFields()
        {
            var sut = new SutClass(2, 42, 4, null);

            Check.That(sut).Considering().NonPublic.Fields.IsEqualTo(new SutClass(2, 42, 4, null));
            Check.ThatCode(() =>
                {Check.That(sut).Considering().NonPublic.Fields.IsEqualTo(new SutClass(2, 42, 4, sut));})
                .FailsWithMessage("", "The checked value's autoproperty 'ThePrivateProperty' (field '<ThePrivateProperty>k__BackingField') does not have the expected value.", "The checked value:", "\t[null]", "The expected value:", "\t[NFluent.Tests.ConsideringShould+SutClass] of type: [NFluent.Tests.ConsideringShould+SutClass]");
            Check.ThatCode(() =>
                {
                    Check.That(new SutClass(2, 42, 4, sut)).Considering().NonPublic.Fields.IsEqualTo(sut);})
                .FailsWithMessage("", "The checked value's autoproperty 'ThePrivateProperty' (field '<ThePrivateProperty>k__BackingField') does not have the expected value.", "The checked value:", "\t[NFluent.Tests.ConsideringShould+SutClass]", "The expected value:", "\t[null]");
        }

        [Test]
        public void WorkWithDeepExclusions()
        {
            var sut = new SutClass(2, 42, 4, new SutClass(1, 2));

            Check.That(sut).Considering().All.Fields
                .Excluding("ThePrivateProperty.thePrivateField", "ThePrivateProperty.TheField")
                .IsEqualTo(new SutClass(2, 42, 4, new SutClass(2, 2)));
        }

        [Test]
        public void WorkWithExclusions()
        {
            var sut = new SutClass(2, 42, 3, null);

            Check.That(sut).Considering().All.Fields.Excluding("thePrivateField")
                .IsEqualTo(new SutClass(2, 42, 4, null));
        }

        [Test]
        public void WorkOnDifferentArray()
        {
            var sut = new {arrayOfInts = new int[4]};
            var expected = new {arrayOfInts = new int[4]};
             Check.That(sut).Considering().NonPublic.Fields.IsEqualTo(expected);
        }

        [Test]
        public void FailOnDifferentArray()
        {
            var sut = new {arrayOfInts = new int[4]};
            var expected = new {arrayOfInts = new int[5]};
            Check.ThatCode(() => { Check.That(sut).Considering().NonPublic.Fields.IsEqualTo(expected); })
                .FailsWithMessage("", "The checked value's field 'arrayOfInts' does not have the expected value.", "The checked value:", "\t[0, 0, 0, 0]", "The expected value:", "\t[0, 0, 0, 0, 0]");
            Check.ThatCode(() => { Check.That(new {arrayOfInts =  "INTS"}).Considering().NonPublic.Fields.IsEqualTo(expected); })
                .FailsWithMessage("", "The checked value's field 'arrayOfInts' does not have the expected value.", "The checked value:", "\t[\"INTS\"] of type: [string]", "The expected value:", "\t[0, 0, 0, 0, 0] of type: [int[]]");
        }

        [Test]
        public void
        WorkForHasSameValue()
        {
            var sut = new SutClass(12, 13);
            Check.That(sut).Considering().Public.Properties.HasSameValueAs(new SutClass(11, 13));
            Check.ThatCode(() =>
                {
                    Check.That(sut).Considering().Public.Properties.HasSameValueAs(new SutClass(11, 12));
                }).FailsWithMessage("", "The checked value is different from the expected one.", "The checked value:", "\t[{ TheProperty = 13 }] of type: [NFluent.Helpers.ReflectionWrapper]", "The expected value: equals to (using operator==)", "\t[NFluent.Tests.ConsideringShould+SutClass] of type: [NFluent.Tests.ConsideringShould+SutClass]");
        }

        [Test]
        public void
        WorkForHasNotSameValue()
        {
            var sut = new SutClass(12, 13);
            Check.That(sut).Considering().Public.Properties.HasDifferentValueThan(new SutClass(11, 12));
        }

        [Test]
        public void
            WorkForIsOneOf()
        {
            var sut = new SutClass(12, 13);
            Check.That(sut).Considering().Public.Properties.IsOneOf(new {TheProperty = 12}, new {TheProperty = 13});
        }

        [Test]
        public void
            FailIfNoneOf()
        {
            var sut = new SutClass(12, 13);
            Check.ThatCode(()=>
            {
               Check.That(sut).Considering().Public.Properties.IsOneOf(new {TheProperty = 12}, new {TheProperty = 14});
            }).FailsWithMessage("", "The checked value is equal to none of the expected value(s) whereas it should.", "The checked value:", "\t[{ TheProperty = 13 }]", "The expected value(s):", "\t[{ TheProperty = 12 }, { TheProperty = 14 }]");
        }

        [Test]
        public void
            WorkForIsNull()
        {
            var sut = new SutClass(5, 7);
            Check.That(sut).Considering().NonPublic.Properties.IsNull();
            Check.ThatCode(() => { Check.That(sut).Considering().NonPublic.Properties.Not.IsNull(); })
                .FailsWithMessage("", "The checked value has only null member, whereas it should not.");
            Check.ThatCode(() => { Check.That(sut).Considering().Public.Properties.IsNull();})
                .FailsWithMessage("", "The checked value has a non null member, whereas it should not.", "The checked value:", "\t[7]");
        }

        [Test]
        public void
            WorkForIsNotNull()
        {
            var sut = new SutClass(5, 7);
            Check.That(sut).Considering().Public.Properties.IsNotNull();
            Check.ThatCode(() => { Check.That(sut).Considering().Public.Properties.Not.IsNotNull(); }).
                FailsWithMessage("", "The checked value has no null member, whereas it should.", "The checked value:", "\t[{ TheProperty = 7 }]");
            Check.ThatCode(() => { Check.That(sut).Considering().NonPublic.Properties.IsNotNull();})
                .FailsWithMessage("", "The checked value has a null member, whereas it should not.");
        }

        [Test]
        public void
            WorkForIsNotNullOnRecursive()
        {
            Check.That(new Recurse()).Considering().All.Fields.IsNotNull();
        }

        [Test]
        public void
            WorkForIsSameReference()
        {
            var sharedReference = new object();
 
            Check.That(new {Property = sharedReference}).Considering().Public.Properties
                .IsSameReferenceAs(new {Property = sharedReference});
            Check.ThatCode(() =>
            {
                Check.That(new {Property = sharedReference}).Considering().Public.Properties
                    .IsSameReferenceAs(new {Property = (object) null});
            }).FailsWithMessage("", "The checked value's property 'Property' does not reference the expected instance.", "The checked value:", "\t[{  }]", "The expected value:", "\t[null]");

            Check.ThatCode(() =>
            {
                Check.That(new {Properties = (object) null}).Considering().Public.Properties
                    .IsSameReferenceAs(new {Property = sharedReference, Properties = (object) null});
            }).FailsWithMessage("", "The expected one's property 'Property' is absent from the checked value.", "The expected property 'Property':", "\t[System.Object]");

            Check.ThatCode(() =>
            {
                Check.That(new {Property = sharedReference, Properties = 2}).Considering().Public.Properties
                    .IsSameReferenceAs(new {Property = sharedReference});
            }).FailsWithMessage("", "The checked value's property 'Properties' is absent from the expected one.", "The value property 'Properties':", "\t[2]");

        }

        [Test]
        public void
            WorkForIsDistinct()
        {
            var sharedReference = new object();

            Check.That(new {Property = sharedReference}).Considering().Public.Properties
                .IsDistinctFrom(new {Property = new object()});
            Check.ThatCode(() =>
            {
                Check.That(new {Property = sharedReference}).Considering().Public.Properties
                    .IsDistinctFrom(new {Property = sharedReference});
            }).FailsWithMessage(
                "", "The checked value's property 'Property' does reference the reference instance, whereas it should not.", "The checked value:", "\t[{  }]");

            Check.ThatCode(() =>
            {
                Check.That(new {Properties = (object) null}).Considering().Public.Properties
                    .IsDistinctFrom(new {Property = sharedReference, Properties = (object) null});
            }).FailsWithMessage("", "The expected one's property 'Property' is absent from the checked value.", "The expected property 'Property':", "\t[System.Object]");

            Check.ThatCode(() =>
            {
                Check.That(new {Property = sharedReference, Properties = 2}).Considering().Public.Properties
                    .IsDistinctFrom(new {Property = new object()});
            }).FailsWithMessage("", "The checked value's property 'Properties' is absent from the expected one.", "The value property 'Properties':", "\t[2]");
        }

        private class Recurse
        {
            private Recurse me;
            private int x = 2;

            public Recurse()
            {
                this.me = this;
            }
        }
        // GH #219
        public class Parent
        {
            public virtual string AutoProperty { get; set; }
        }

        public class Child : Parent
        {
            public override string AutoProperty { get; set; }
        }

        [Test]
        public void HandleOverringForFields()
        {
            // Arrange
            var autoPropertyValue = "I am a test.";
            var childOne = new Child { AutoProperty = autoPropertyValue };

            // Act
            var childTwo = new Child { AutoProperty = autoPropertyValue };

            // Assert
            Check.That(childOne).HasFieldsWithSameValues(childTwo);
        }

        [Test]
        public void HandleOverringForProperties()
        {
            // Arrange
            var autoPropertyValue = "I am a test.";
            var childOne = new Child { AutoProperty = autoPropertyValue };

            // Act
            var childTwo = new Child { AutoProperty = autoPropertyValue };

            // Assert
            Check.That(childOne).Considering().Public.Properties.IsEqualTo(childTwo);
        }

    }
}