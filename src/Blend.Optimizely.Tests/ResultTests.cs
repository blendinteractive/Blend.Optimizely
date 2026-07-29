using System;
using Blend.Optimizely;
using Xunit;

namespace Blend.Optimizely.Tests
{
    public class ResultTests
    {
        #region Static factories
        [Fact]
        public void Success_SetsIsSuccessfulTrue()
        {
            var result = Result.Success<string, int>("hello");
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void Success_SetsValue()
        {
            var result = Result.Success<string, int>("hello");
            Assert.Equal("hello", result.Value);
        }

        [Fact]
        public void Success_SetsErrorToDefault()
        {
            var result = Result.Success<string, int>("hello");
            Assert.Equal(default, result.Error);
        }

        [Fact]
        public void Error_SetsIsSuccessfulFalse()
        {
            var result = Result.Error<string, int>(42);
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public void Error_SetsError()
        {
            var result = Result.Error<string, int>(42);
            Assert.Equal(42, result.Error);
        }

        [Fact]
        public void Error_SetsValueToDefault()
        {
            var result = Result.Error<string, int>(42);
            Assert.Null(result.Value);
        }
        #endregion

        #region Implicit conversions
        [Fact]
        public void ImplicitFromValue_IsSuccessful()
        {
            Result<string, int> result = "hello";
            Assert.True(result.IsSuccessful);
            Assert.Equal("hello", result.Value);
        }

        [Fact]
        public void ImplicitFromError_IsNotSuccessful()
        {
            Result<string, int> result = 42;
            Assert.False(result.IsSuccessful);
            Assert.Equal(42, result.Error);
        }
        #endregion

        #region HasSuccessValue
        [Fact]
        public void HasSuccessValue_ReturnsTrueAndValue_WhenSuccessful()
        {
            var result = Result.Success<string, int>("hello");
            Assert.True(result.HasSuccessValue(out var value));
            Assert.Equal("hello", value);
        }

        [Fact]
        public void HasSuccessValue_ReturnsFalse_WhenError()
        {
            var result = Result.Error<string, int>(42);
            Assert.False(result.HasSuccessValue(out var value));
            Assert.Null(value);
        }

        [Fact]
        public void HasSuccessValue_ReturnsFalse_WhenSuccessfulButValueIsNull()
        {
            // IsSuccessful=true with a null Value — HasSuccessValue still returns false.
            var result = new Result<string, int>(null, default, true);
            Assert.False(result.HasSuccessValue(out _));
        }
        #endregion

        #region HasError
        [Fact]
        public void HasError_ReturnsTrueAndError_WhenError()
        {
            var result = Result.Error<string, int>(42);
            Assert.True(result.HasError(out var error));
            Assert.Equal(42, error);
        }

        [Fact]
        public void HasError_ReturnsFalse_WhenSuccessful()
        {
            var result = Result.Success<string, int>("hello");
            Assert.False(result.HasError(out _));
        }
        #endregion

        #region Assume
        [Fact]
        public void Assume_ReturnsValue_WhenSuccessful()
        {
            var result = Result.Success<string, int>("hello");
            Assert.Equal("hello", result.Assume());
        }

        [Fact]
        public void Assume_Throws_WhenError()
        {
            var result = Result.Error<string, int>(42);
            Assert.Throws<InvalidOperationException>(() => result.Assume());
        }

        [Fact]
        public void Assume_UsesErrorToString_WhenNoMessageProvided()
        {
            var result = Result.Error<string, int>(42);
            var ex = Assert.Throws<InvalidOperationException>(() => result.Assume());
            Assert.Equal("42", ex.Message);
        }

        [Fact]
        public void Assume_UsesCustomMessage_WhenProvided()
        {
            var result = Result.Error<string, int>(42);
            var ex = Assert.Throws<InvalidOperationException>(() => result.Assume(e => $"Error code: {e}"));
            Assert.Equal("Error code: 42", ex.Message);
        }
        #endregion

        #region Switch
        [Fact]
        public void Switch_CallsOnSuccess_WhenSuccessful()
        {
            var result = Result.Success<string, int>("hello");
            string? captured = null;
            result.Switch(onError: _ => { }, onSuccess: v => captured = v);
            Assert.Equal("hello", captured);
        }

        [Fact]
        public void Switch_CallsOnError_WhenError()
        {
            var result = Result.Error<string, int>(42);
            int captured = 0;
            result.Switch(onError: e => captured = e, onSuccess: _ => { });
            Assert.Equal(42, captured);
        }

        [Fact]
        public void Switch_DoesNotCallOnError_WhenSuccessful()
        {
            var result = Result.Success<string, int>("hello");
            bool errorCalled = false;
            result.Switch(onError: _ => errorCalled = true, onSuccess: _ => { });
            Assert.False(errorCalled);
        }

        [Fact]
        public void Switch_DoesNotCallOnSuccess_WhenError()
        {
            var result = Result.Error<string, int>(42);
            bool successCalled = false;
            result.Switch(onError: _ => { }, onSuccess: _ => successCalled = true);
            Assert.False(successCalled);
        }

        [Fact]
        public void Switch_Throws_WhenSuccessfulButValueIsNull()
        {
            var result = new Result<string, int>(null, default, true);
            Assert.Throws<NotImplementedException>(() =>
                result.Switch(onError: _ => { }, onSuccess: _ => { }));
        }

        [Fact]
        public void Switch_Throws_WhenNotSuccessfulButErrorIsDefault()
        {
            var result = new Result<string, string?>(null, null, false);
            Assert.Throws<NotImplementedException>(() =>
                result.Switch(onError: _ => { }, onSuccess: _ => { }));
        }
        #endregion

        #region Match
        [Fact]
        public void Match_ReturnsOnSuccess_WhenSuccessful()
        {
            var result = Result.Success<string, int>("hello");
            var output = result.Match(onError: e => $"err:{e}", onSuccess: v => $"ok:{v}");
            Assert.Equal("ok:hello", output);
        }

        [Fact]
        public void Match_ReturnsOnError_WhenError()
        {
            var result = Result.Error<string, int>(42);
            var output = result.Match(onError: e => $"err:{e}", onSuccess: v => $"ok:{v}");
            Assert.Equal("err:42", output);
        }

        [Fact]
        public void Match_Throws_WhenSuccessfulButValueIsNull()
        {
            var result = new Result<string, int>(null, default, true);
            Assert.Throws<NotImplementedException>(() =>
                result.Match(onError: e => "e", onSuccess: v => "v"));
        }

        [Fact]
        public void Match_Throws_WhenNotSuccessfulButErrorIsNull()
        {
            var result = new Result<string, string?>(null, null, false);
            Assert.Throws<NotImplementedException>(() =>
                result.Match(onError: e => "e", onSuccess: v => "v"));
        }
        #endregion
    }
}
